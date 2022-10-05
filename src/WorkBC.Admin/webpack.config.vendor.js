const path = require("path");
const webpack = require("webpack");
const MiniCssExtractPlugin = require("mini-css-extract-plugin");
const CopyWebpackPlugin = require("copy-webpack-plugin");
const CssMinimizerPlugin = require("css-minimizer-webpack-plugin");

module.exports = (env) => {
    const isDevBuild = !(env && env.prod);
    return [
        {
            stats: {
                modules: false,
                entrypoints: false,
                children: false
            },
            resolve: {
                extensions: [".js"]
            },
            module: {
                rules: [
                    {
                        test: /\.(jpg|jpeg|gif|png)$/,
                        use: {
                            loader: "file-loader",
                            options: {
                                name: 'images/[name].[ext]',
                            }
                        }
                    },
                    {
                        test: /\.(woff|woff2|eot|ttf|svg)$/,
                        use: {
                            loader: "file-loader",
                            options: {
                                name: 'fonts/[name].[ext]',
                            }
                        }
                    },
                    {
                        test: /\.css(\?|$)/,
                        use: [
                            {
                                loader: MiniCssExtractPlugin.loader,
                                options: {}
                            }, 'css-loader'
                        ]
                    }

                ]
            },
            entry: {
                vendor: [
                    "bootstrap/dist/css/bootstrap.css",
                    "select2/dist/css/select2.css",
                    "select2-bootstrap-theme/dist/select2-bootstrap.css",
                    "font-awesome/css/font-awesome.css",
                    "bootstrap-datepicker/dist/css/bootstrap-datepicker3.css"
                ]
            },
            output: {
                path: path.join(__dirname, "wwwroot", "dist"),
                publicPath: "/dist/",
                filename: "[name].js",
                library: "[name]_[fullhash]",
            },
			performance: {
				hints: process.env.NODE_ENV === 'production' ? "warning" : false
			},
            optimization: {
                minimize: true,
                minimizer: [
                    new CssMinimizerPlugin(),
                ],
            },
            plugins: [
                new MiniCssExtractPlugin({
                     filename: "css/vendor.min.css"
                }),
                new webpack.DllPlugin({
                    path: path.join(__dirname, "wwwroot", "dist", "[name]-manifest.json"),
                    name: "[name]_[fullhash]"
                }),
                new webpack.ProvidePlugin({
                    $: "jquery",
                    "window.$": "jquery",
                    jQuery: "jquery",
                    "window.jQuery": "jquery"
                }),
                new CopyWebpackPlugin({
                    patterns: [
                        {
                            from: "node_modules/jquery/dist/jquery+(.min|).js",
                            to: "lib/[name].js",
                            toType: "template"
                        },
                        {
                            from: "node_modules/bootstrap/dist/js/bootstrap.bundle+(.min|).js",
                            to: "lib/[name].js",
                            toType: "template"
                        },
                        {
                            // the min version is packaged above.
                            // Just copy the regular version for Development
                            from: "node_modules/bootstrap/dist/css/bootstrap.css",
                            to: "css/bootstrap.css",
                            toType: "file"
                        },
                        {
                            from: "node_modules/select2/dist/js/select2+(.full|)+(.min|).js",
                            to: "lib/[name].js"
                        },
                        {
                            // the min version is packaged above.
                            // Just copy the regular version for Development
                            from: "node_modules/select2/dist/css/select2.css",
                            to: "css/select2.css",
                            toType: "file"
                        },
                        {
                            // the min version is packaged above.
                            // Just copy the regular version for Development
                            from: "node_modules/select2-bootstrap-theme/dist/select2-bootstrap.css",
                            to: "css/select2-bootstrap.css",
                            toType: "file"
                        },
                        {
                            from: "node_modules/@bcgov/bc-sans/fonts/*",
                            to: "fonts/[name].[ext]"
                        },
                        {
                            from: "node_modules/font-awesome/css/font-awesome.css",
                            to: "css/font-awesome.css",
                            toType: "file"
                        },
                        {
                            from: "node_modules/font-awesome/fonts/*",
                            to: "fonts/[name].[ext]"
                        },
                        {
                            from: "node_modules/datatables.net/js/jquery.dataTables+(.min|).js",
                            to: "lib/[name].js"
                        },
                        {
                            from: "node_modules/datatables.net-bs/js/dataTables.bootstrap+(.min|).js",
                            to: "lib/[name].js"
                        },
                        {
                            from: "node_modules/datatables.net-bs/css/dataTables.bootstrap+(.min|).css",
                            to: "css/[name].css"
                        },
                        {
                            from: "node_modules/codemirror/lib/codemirror.css",
                            to: "css/codemirror.css",
                            toType: "file"
                        },
                        {
                            from: "node_modules/codemirror/lib/codemirror.js",
                            to: "lib/codemirror.js",
                            toType: "file"
                        },
                        {
                            from: "node_modules/codemirror/mode/htmlmixed/htmlmixed.js",
                            to: "lib/codemirror-htmlmixed.js",
                            toType: "file"
                        },
                        {
                            from: "node_modules/codemirror/mode/xml/xml.js",
                            to: "lib/codemirror-xml.js",
                            toType: "file"
                        },
                        {
                            from: "node_modules/moment/min/moment.min.js",
                            to: "lib/moment.min.js",
                            toType: "file"
                        },
                        {
                            from: "node_modules/daterangepicker/daterangepicker.js",
                            to: "lib/daterangepicker.js"
                        },
                        {
                            from: "node_modules/daterangepicker/daterangepicker.css",
                            to: "css/daterangepicker.css"
                        },
                        {
                            from: "node_modules/datatables.net-buttons/js/dataTables.buttons+(.min|).js",
                            to: "lib/[name].js"
                        },
                        {
                            from: "node_modules/datatables.net-buttons/js/buttons.html5+(.min|).js",
                            to: "lib/dataTables-[name].js"
                        },
                        {
                            from: "node_modules/datatables.net-dt/css/jquery.dataTables+(.min|).css",
                            to: "css/[name].css"
                        },
                        {
                            from: "node_modules/datatables.net-dt/images/*",
                            to: "images/[name].[ext]"
                        },
                        {
                            from: "node_modules/datatables.net-buttons-dt/css/buttons.dataTables+(.min|).css",
                            to: "css/[name].css"
                        },
                        {
                            from: "node_modules/jszip/dist/*.js",
                            to: "lib/[name].js"
                        },
                        {
                            // the min version is packaged above.  
                            // Just copy the regular version for Development
                            from: "node_modules/bootstrap-datepicker/dist/css/bootstrap-datepicker3.css",
                            to: "css/bootstrap-datepicker3.css",
                            toType: "file"
                        },
                        {
                            from: "node_modules/bootstrap-datepicker/dist/js/bootstrap-datepicker+(.min|).js",
                            to: "lib/[name].js",
                            toType: "template"
                        }
                    ]
                }),
                new webpack.DefinePlugin({
                    'process.env.NODE_ENV': isDevBuild ? '"development"' : '"production"'
                })
            ]
        }
    ];
};
