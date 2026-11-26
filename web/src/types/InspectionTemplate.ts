export interface InspectionTemplate {
  id: number;
  name: string;
  description: string;
  isActive: boolean;
  createdAt: string;
  versionCount: number;
}

export interface RankedTemplateRecommendation {
  templateId: number;
  templateName: string;
  rank: number;
  confidence: number;
  reasoning: string;
  checklistItemRecommendations: unknown[];
}