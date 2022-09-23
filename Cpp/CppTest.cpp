//
// FixPointCS
//
// Copyright(c) Jere Sanisalo, Petri Kero
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
//
#include <iostream>

#include "FixedUtil.h"
#include "Fixed32.h"
#include "Fixed64.h"

#include "UnitTest.h"

void Test32()
{
	double v = 0.0001f;

	while (v < 150.f)
	{
		Fixed32::FP_INT fv = Fixed32::FromDouble(v);
		Fixed32::FP_INT fv_div = Fixed32::DivPrecise(fv, Fixed32::FromDouble(-2.34));
		Fixed32::FP_INT fv_sqrt = Fixed32::Sqrt(fv);
		Fixed32::FP_INT fv_sin = Fixed32::Sin(fv);
		Fixed32::FP_INT fv_rcp = Fixed32::RcpFast(fv);

		std::cout << v
			<< ": div_by_-2.34: " << Fixed32::ToDouble(fv_div)
			<< ", sqrt: " << Fixed32::ToDouble(fv_sqrt)
			<< ", sin: " << Fixed32::ToDouble(fv_sin)
			<< ", rcp: " << Fixed32::ToDouble(fv_rcp)
			<< std::endl;

		// Next number
		v *= 1.5f;
	}
}

void Test64()
{
	double v = 0.0001f;

	while (v < 150.f)
	{
		Fixed64::FP_LONG fv = Fixed64::FromDouble(v);
		Fixed64::FP_LONG fv_div = Fixed64::DivPrecise(fv, Fixed64::FromDouble(-2.34));
		Fixed64::FP_LONG fv_sqrt = Fixed64::Sqrt(fv);
		Fixed64::FP_LONG fv_sin = Fixed64::Sin(fv);
		Fixed64::FP_LONG fv_rcp = Fixed64::RcpFast(fv);

		std::cout << v
			<< ": div_by_-2.34: " << Fixed64::ToDouble(fv_div)
			<< ", sqrt: " << Fixed64::ToDouble(fv_sqrt)
			<< ", sin: " << Fixed64::ToDouble(fv_sin)
			<< ", rcp: " << Fixed64::ToDouble(fv_rcp)
			<< std::endl;

		// Next number
		v *= 1.5f;
	}
}

int main()
{
	std::cout << "Testing 16.16 fixed point numbers.." << std::endl;
	Test32();

	std::cout << std::endl;
	std::cout << "Testing 32.32 fixed point numbers.." << std::endl;
	Test64();

	std::cout << std::endl;
	std::cout << "Executing all unit tests.." << std::endl;
	UnitTest_TestAll();
	std::cout << "Unit tests finished!" << std::endl;

    return 0;
}
