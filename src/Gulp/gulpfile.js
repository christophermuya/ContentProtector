const { watch, src, dest } = require('gulp');

const sourceFolders = [
	'../ContentProtector.Plugin/App_Plugins/'
];

const destination = '../ContentProtector.Web/App_Plugins/';

function copy(path, baseFolder) {
	return src(path, { base: baseFolder })
		.pipe(dest(destination));
}

function time() {
	return '[' + new Date().toISOString().slice(11, -5) + ']';
}

exports.default = function () {
	console.log('Watching for changes in source folders...');
	sourceFolders.forEach(function (sourceFolder) {
		let source = sourceFolder + '**/*';
		watch(source, { ignoreInitial: false })
			.on('change', function (path, stats) {
				console.log(time(), path, sourceFolder, 'changed');
				copy(path, sourceFolder);
			})
			.on('add', function (path, stats) {
				console.log(time(), path, sourceFolder, 'added');
				copy(path, sourceFolder);
			});
	});
};