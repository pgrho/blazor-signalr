/// <binding AfterBuild='default' Clean='clean' />
var gulp = require("gulp");
var concat = require('gulp-concat');
var uglify = require('gulp-uglify');
var del = require('del');

gulp.task('clean', function () {
    return del(['content/*.js']);
});
gulp.task('scripts', ['clean'], function () {
    gulp.src([
        'Scripts/jquery.signalR-2.4.1.js',
        'Scripts/BlazorShim.js'
    ])
        .pipe(concat('bundle.js'))
        .pipe(uglify())
        .pipe(gulp.dest('content/'));
});
gulp.task('default', ['scripts'], function () { });