/// <binding BeforeBuild='default' Clean='clean' />
var gulp = require("gulp");
var del = require('del');

gulp.task('clean', function () {
    return del(['wwwroot/Shipwreck.BlazorSignalR.js']);
});
gulp.task('bundle', function () {
    return gulp.src([
        '../Shipwreck.BlazorSignalR/wwwroot/Shipwreck.BlazorSignalR.js'
    ]).pipe(gulp.dest('wwwroot/'));
});
gulp.task('default', gulp.series(['clean', 'bundle']));