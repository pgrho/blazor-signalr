/// <binding BeforeBuild='default' Clean='clean' />
var gulp = require("gulp");
var del = require('del');

gulp.task('clean', function () {
    return del(['wwwroot/bundle.js']);
});
gulp.task('bundle', function () {
    return gulp.src([
        '../Shipwreck.BlazorSignalR/wwwroot/bundle.js'
    ]).pipe(gulp.dest('wwwroot/'));
});
gulp.task('default', gulp.series(['clean', 'bundle']));