/// <binding BeforeBuild='default' Clean='clean' />
var gulp = require("gulp");
var concat = require('gulp-concat');
var uglify = require('gulp-uglify');
var del = require('del');
var ts = require('gulp-typescript');

gulp.task('clean', function () {
    return del(['content/*.js']);
});
gulp.task('scripts', function () {

    gulp.src(['Scripts/BlazorShim.ts']).pipe(ts({
        outFile: 'BlazorShim.js'
    })).pipe(gulp.dest('Scripts/'));

    return gulp.src([
        'Scripts/jquery.signalR-2.4.1.js',
        'Scripts/BlazorShim.js'
    ])
        .pipe(concat('bundle.js'))
        .pipe(uglify({
            output: {
                comments: /^!/
            }
        }))
        .pipe(gulp.dest('content/'));
});
gulp.task('default', gulp.series(['clean', 'scripts']));