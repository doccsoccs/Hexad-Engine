#pragma once

#include "Test.h"
#include "..\Engine\Components\Entity.h"
#include "..\Engine\Components\Transform.h"
using namespace hexad;

class engine_test : public test
{
public:
	bool initialize() override { return true; }
	bool run() override { return true; }
	bool shutdown() override { return true; }
};