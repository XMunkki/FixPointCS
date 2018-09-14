#pragma once
#ifndef __UNITTEST_H
#define __UNITTEST_H

#include <stdio.h>
#include <inttypes.h>

#include "Fixed32.h"
#include "Fixed64.h"

class Util
{
public:
	static void Check(const char* opName, int64_t output, int64_t expected, int64_t input0)
	{
		if (output != expected)
			printf("MISMATCH in %s: got %" PRId64 ", expected %" PRId64 ", inputs %" PRId64 "\n", opName, output, expected, input0);
	}

	static void Check(const char* opName, int64_t output, int64_t expected, int64_t input0, int64_t input1)
	{
		if (output != expected)
			printf("MISMATCH in %s: got %" PRId64 ", expected %" PRId64 ", inputs %" PRId64 " %" PRId64 "\n", opName, output, expected, input0, input1);
	}

	// static void Check(const char* opName, int32_t output, int32_t expected, int32_t input0)
	// {
	// 	if (output != expected)
	// 		printf("MISMATCH in %s: got %d, expected %d, inputs %d\n", opName, output, expected, input0);
	// }
	// 
	// static void Check(const char* opName, int32_t output, int32_t expected, int32_t input0, int32_t input1)
	// {
	// 	if (output != expected)
	// 		printf("MISMATCH in %s: got %d, expected %d, inputs %d %d\n", opName, output, expected, input0, input1);
	// }
};

void UnitTest_TestAll();

#endif
