module.exports = {
  presets: [
    ['@babel/preset-env', { targets: { node: 'current' } }],
    ['@babel/preset-react', { runtime: 'automatic' }],
    '@babel/preset-typescript',
  ],
  plugins: [
    function importMetaPlugin() {
      return {
        visitor: {
          MetaProperty(path) {
            // Transforms import.meta → ({ env: {} })
            // So import.meta.env.VITE_API_URL → undefined, ?? '' → ''
            path.replaceWithSourceString('({ env: {} })');
          },
        },
      };
    },
  ],
};