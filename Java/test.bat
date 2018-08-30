@javac -d classes *.java || exit /b !ERRORLEVEL!
@java -cp classes fixpointcs.test.FixPointTest
