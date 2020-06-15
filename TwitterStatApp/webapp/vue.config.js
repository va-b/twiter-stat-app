module.exports = {
  "lintOnSave": false,
  "productionSourceMap": false,

  "chainWebpack": config => {
    config.optimization
          .minimizer('terser')
          .tap(args => {
            const { terserOptions } = args[0]
            terserOptions.keep_classnames = true
            terserOptions.keep_fnames = true
            return args
          })
  },

  "devServer": {
    "disableHostCheck": true,
    "host": "localhost",
    "port": 80,
    "proxy": {
      "^/api": {
        "target": "http://localhost:5000",
        "changeOrigin": true
      }
    }
  },

  "transpileDependencies": [
    "vuetify"
  ],

  outputDir: '../wwwroot',
  productionSourceMap: false
}