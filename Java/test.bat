@javac -d classes FixPointTest.java Fixed32.java Fixed64.java || exit /b !ERRORLEVEL!
@java -cp classes fixpointtest.FixPointTest
