const path = require("path");
const fs = require("fs");
const MiniCssExtractPlugin = require('mini-css-extract-plugin');
const CssMinimizerPlugin = require("css-minimizer-webpack-plugin");
const RemovePlugin = require('remove-files-webpack-plugin');
const bundleOutputDir = "./wwwroot/dist";

var appBasePath = "./";

var entries = {};

// any scss file that doesn't start with an underscore gets built into 
// a css file with the same name
fs.readdirSync(appBasePath + "Styles/").forEach(function (file) {
    if (!file.startsWith("_")) {
        var name = file.replace(".scss", "");
        entries["/css/" + name] = appBasePath + "Styles/" + file;
    }
});

module.exports = (env) => {
    const isDevBuild = !(env && env.prod);
    return [{
        stats: {
            modules: false,
            children: false
        },
        resolve: {
            extensions: ['.scss']
        },
        optimization: {
            minimize: !isDevBuild,
            minimizer: [
                new CssMinimizerPlugin(),
            ],
        },
        entry: entries,
        output: {
            path: path.join(__dirname, bundleOutputDir),
            filename: "[name]"
        },
        module: {
            rules: [
                {
                    test: /\.scss/,
                    use: [
                        {
                            loader: MiniCssExtractPlugin.loader,
                        },
                        'css-loader',
                        'sass-loader'
                    ] }
            ]
        },
        plugins: [
            new MiniCssExtractPlugin(),
            new RemovePlugin({
                after: {
                    root: './wwwroot/dist/css',
                    include: [],
                    test: [
                        {
                            folder: './',
                            method: (absoluteItemPath) => {
                                return !(new RegExp(/\.css$/, 'm').test(absoluteItemPath));
                            }
                        }
                    ],
                }
            })
        ]
    }];
};
