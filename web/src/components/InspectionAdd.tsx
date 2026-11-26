import { useEffect, useState, type FormEvent, type ChangeEvent } from 'react';
import { Link } from 'react-router-dom';
import { createInspection, uploadInspectionImage } from '../api/inspectionsApi';
import { fetchTemplates } from '../api/templatesApi';
import type { InspectionTemplate } from '../types/InspectionTemplate';

const MAX_IMAGES = 3;

export function InspectionAdd() {
  const [templates, setTemplates] = useState<InspectionTemplate[]>([]);
  const [selectedTemplateId, setSelectedTemplateId] = useState<number | null>(null);
  const [isLoadingTemplates, setIsLoadingTemplates] = useState(true);
  const [templateError, setTemplateError] = useState<string | null>(null);
  const [showFields, setShowFields] = useState(false);
  const [title, setTitle] = useState('');
  const [description, setDescription] = useState('');
  const [scheduledAt, setScheduledAt] = useState('');
  const [images, setImages] = useState<File[]>([]);
  const [message, setMessage] = useState<string | null>(null);
  const [imageError, setImageError] = useState<string | null>(null);
  const [imagePreviewUrls, setImagePreviewUrls] = useState<string[]>([]);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [createdRecordId, setCreatedRecordId] = useState<string | null>(null);

  useEffect(() => {
    let isMounted = true;

    async function loadTemplates() {
      setIsLoadingTemplates(true);
      setTemplateError(null);
      try {
        const availableTemplates = await fetchTemplates();
        if (isMounted) {
          setTemplates(availableTemplates);
        }
      } catch (error) {
        if (isMounted) {
          setTemplateError(
            error instanceof Error ? error.message : 'Could not load templates.',
          );
        }
      } finally {
        if (isMounted) {
          setIsLoadingTemplates(false);
        }
      }
    }

    void loadTemplates();
    return () => {
      isMounted = false;
    };
  }, []);

  useEffect(() => {
    const previewUrls = images.map((image) => URL.createObjectURL(image));
    setImagePreviewUrls(previewUrls);

    return () => previewUrls.forEach((previewUrl) => URL.revokeObjectURL(previewUrl));
  }, [images]);

  function handleImageChange(event: ChangeEvent<HTMLInputElement>) {
    const selectedFiles = Array.from(event.target.files ?? []);
    setMessage(null);
    setImageError(null);

    const validFiles = selectedFiles.filter((file) => ['image/png', 'image/jpeg', 'image/gif', 'image/webp'].includes(file.type) && file.size <= 10 * 1024 * 1024);
    const remainingSlots = MAX_IMAGES - images.length;
    const filesToAdd = validFiles.slice(0, remainingSlots);
    const errors: string[] = [];

    if (validFiles.length !== selectedFiles.length) {
      errors.push('Select PNG, JPEG, GIF, or WebP images up to 10 MB.');
    }
    if (validFiles.length > remainingSlots) {
      errors.push(`You can add up to ${MAX_IMAGES} images.`);
    }

    if (filesToAdd.length > 0) {
      setImages((currentImages) => [...currentImages, ...filesToAdd]);
    }
    if (errors.length > 0) {
      setImageError(errors.join(' '));
    }
    event.target.value = '';
  }

  function removeImage(indexToRemove: number) {
    setImages((currentImages) =>
      currentImages.filter((_, index) => index !== indexToRemove),
    );
    setMessage(null);
    setImageError(null);
  }

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setMessage(null);
    setIsSubmitting(true);
    let recordId = createdRecordId;

    try {
      if (!recordId) {
        recordId = await createInspection({
          templateId: selectedTemplateId,
          title: title.trim(),
          description: description.trim(),
          scheduledAt: new Date(scheduledAt).toISOString(),
        });
        setCreatedRecordId(recordId);
      }

      const pendingImages = [...images];
      while (pendingImages.length > 0) {
        await uploadInspectionImage(recordId, pendingImages[0]);
        pendingImages.shift();
        setImages([...pendingImages]);
      }

      setMessage(`Inspection ${recordId} was saved${images.length ? ' and its images were uploaded' : ''}.`);
      setCreatedRecordId(null);
      setTitle('');
      setDescription('');
      setScheduledAt('');
      setImages([]);
    } catch (submitError) {
      const detail =
        submitError instanceof Error ? submitError.message : String(submitError);
      setMessage(
        recordId
          ? `Inspection ${recordId} was saved, but image upload failed: ${detail}. Submit again to retry the image upload.`
          : detail,
      );
    } finally {
      setIsSubmitting(false);
    }
  }

  return (
    <section className="inspection-form-page">
      <header className="inspection-form-page__header">
        <div>
          <p className="eyebrow">Inspections</p>
          <h1>Add inspection</h1>
          <p className="subtitle">Enter inspection details and optionally select an image.</p>
        </div>
        <Link to="/" className="inspection-form-page__back">Back to inspections</Link>
      </header>

      <form className="inspection-form" onSubmit={handleSubmit}>
        <fieldset disabled={isSubmitting}>
        {!showFields ? (
          <section className="inspection-form__step" aria-labelledby="template-step-heading">
            <h2 id="template-step-heading">Select a template</h2>
            <div className="inspection-form__template-controls">
              <div className="inspection-form__field">
                <label htmlFor="inspection-template">Template</label>
                <select
                  id="inspection-template"
                  name="template"
                  value={selectedTemplateId ?? ''}
                  onChange={(event) => {
                    setSelectedTemplateId(
                      event.target.value ? Number(event.target.value) : null,
                    );
                  }}
                  disabled={isLoadingTemplates || templates.length === 0}
                  required
                >
                  <option value="">
                    {isLoadingTemplates ? 'Loading templates…' : 'Choose a template'}
                  </option>
                  {templates.map((template) => (
                    <option key={template.id} value={template.id}>
                      {template.name}
                    </option>
                  ))}
                </select>
                {templateError && (
                  <p className="inspection-form__error" role="alert">{templateError}</p>
                )}
                {!isLoadingTemplates && !templateError && templates.length === 0 && (
                  <p className="inspection-form__message">No templates are available.</p>
                )}
              </div>
              <div className="inspection-form__field">
                <label htmlFor="template-images">Inspection image (optional)</label>
                <input
                  id="template-images"
                  name="templateImages"
                  type="file"
                  accept="image/png,image/jpeg,image/gif,image/webp"
                  multiple
                  disabled={images.length >= MAX_IMAGES}
                  onChange={handleImageChange}
                />
                <span>{images.length} of {MAX_IMAGES} images selected</span>
                <p className="inspection-form__hint">
                  Images are attached to the inspection when it is saved.
                </p>
                {images.length > 0 && (
                  <div className="inspection-form__first-image">
                    <img src={imagePreviewUrls[0]} alt={`Preview of ${images[0].name}`} />
                  </div>
                )}
                {imageError && (
                  <p className="inspection-form__error" role="alert">{imageError}</p>
                )}
              </div>
            </div>
            <button
              type="button"
              className="inspection-form__submit"
              onClick={() => setShowFields(true)}
              disabled={selectedTemplateId === null}
            >
              Next
            </button>
          </section>
        ) : (
          <>
        <section className="inspection-form__step" aria-labelledby="fields-step-heading">
          <div className="inspection-form-page__header">
            <div>
              <h2 id="fields-step-heading">Inspection fields</h2>
              <p className="subtitle">
                Template: {templates.find((template) => template.id === selectedTemplateId)?.name}
              </p>
            </div>
            <button
              type="button"
              className="inspection-form-page__back"
              onClick={() => setShowFields(false)}
            >
              Change template
            </button>
          </div>
        </section>
        <div className="inspection-form__field">
          <label htmlFor="inspection-title">Title</label>
          <input
            id="inspection-title"
            name="title"
            type="text"
            value={title}
            onChange={(event) => {
              setTitle(event.target.value);
              setMessage(null);
            }}
            required
            maxLength={200}
          />
        </div>

        <div className="inspection-form__field">
          <label htmlFor="inspection-description">Description</label>
          <textarea
            id="inspection-description"
            name="description"
            value={description}
            onChange={(event) => {
              setDescription(event.target.value);
              setMessage(null);
            }}
            maxLength={2000}
            rows={5}
          />
        </div>

        <div className="inspection-form__field">
          <label htmlFor="inspection-scheduled-at">Scheduled date and time</label>
          <input
            id="inspection-scheduled-at"
            name="scheduledAt"
            type="datetime-local"
            value={scheduledAt}
            onChange={(event) => {
              setScheduledAt(event.target.value);
              setMessage(null);
            }}
            required
          />
        </div>

        <div className="inspection-form__field">
          <label htmlFor="inspection-images">Inspection images (optional, up to {MAX_IMAGES})</label>
          <input
            id="inspection-images"
            name="images"
            type="file"
            accept="image/png,image/jpeg,image/gif,image/webp"
            multiple
            disabled={images.length >= MAX_IMAGES}
            onChange={handleImageChange}
          />
          <span>{images.length} of {MAX_IMAGES} images selected</span>
          {imageError && <p className="inspection-form__error" role="alert">{imageError}</p>}
          {images.length > 0 && (
            <div className="inspection-form__previews">
              {images.map((image, index) => (
                <div className="inspection-form__preview" key={`${image.name}-${image.lastModified}-${index}`}>
                  <img src={imagePreviewUrls[index]} alt={`Preview of ${image.name}`} />
                  <div>
                    <span>{image.name}</span>
                    <button
                      type="button"
                      className="inspection-form__remove-image"
                      onClick={() => removeImage(index)}
                    >
                      Remove image
                    </button>
                  </div>
                </div>
              ))}
            </div>
          )}
        </div>

        <p className="inspection-form__note">
          The inspection is saved first. Selected images are then uploaded to that inspection.
        </p>
        {message && <p className="inspection-form__message" role="status">{message}</p>}
        <button type="submit" className="inspection-form__submit" disabled={isSubmitting}>
          {isSubmitting
            ? (createdRecordId ? 'Uploading images…' : 'Saving inspection…')
            : createdRecordId ? 'Retry image upload' : 'Create inspection'}
        </button>
          </>
        )}
        </fieldset>
      </form>
    </section>
  );
}
