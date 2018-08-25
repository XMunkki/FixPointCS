package fixpointtest;

import fixpointcs.Fixed32;
import fixpointcs.Fixed64;

public class FixPointTest
{
	public static void TestMul(double a, double b)
	{
		System.out.printf("%f vs %f%n", Fixed64.ToDouble(Fixed64.Mul(Fixed64.FromDouble(a), Fixed64.FromDouble(b))), a*b);
	}

	public static void main(String[] args)
	{
		TestMul(1.0, 1.0);
		TestMul(0.9999, 0.99999);
		TestMul(1000.0, -1000.0);
		TestMul(1234567.0, -0.000123123);
		TestMul(99999.99999, -140.24928498);

		System.out.printf("rcp(1.0001) == %f", Fixed32.ToDouble(Fixed32.Rcp(Fixed32.FromDouble(1.0001))));
	}
}
