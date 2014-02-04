module.exports = function(grunt) {
    grunt.loadNpmTasks('grunt-msbuild');
    grunt.loadNpmTasks('grunt-dotnet-assembly-info');
    grunt.loadNpmTasks('grunt-nunit-runner');

    grunt.registerTask('default', ['msbuild', 'nunit']);
    grunt.registerTask('deploy', ['assemblyinfo', 'msbuild', 'nunit']);

    grunt.initConfig({
        assemblyinfo: {
            options: {
                files: ['src/Flexo.sln'],
                info: {
                    version: process.env.BUILD_NUMBER, 
                    fileVersion: process.env.BUILD_NUMBER
                }
            }
        },
        msbuild: {
            src: ['src/Flexo.sln'],
            options: {
                projectConfiguration: 'Release',
                targets: ['Clean', 'Rebuild'],
                stdout: true
            }
        },
        nunit: {
            options: {
                files: ['src/Flexo.sln'],
                teamcity: true
            }
        }
    });
}