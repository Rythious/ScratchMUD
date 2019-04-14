const path = require("path");

module.exports = {
    entry: {
        server: './wwwroot/ts/serverevents.ts'
    },
    resolve: {
        extensions: [".js", ".ts"]
    },
    devtool: 'inline-source-map',
    module: {
        rules: [
            {
                test: /\.ts?$/,
                use: 'ts-loader',
                exclude: /node_modules/
            }
        ]
    },
    output: {
        filename: '[name].js',
        path: path.resolve(__dirname, 'wwwroot/dist/js/')
    }
};