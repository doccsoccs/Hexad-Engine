#pragma once

class test
{
public:
	virtual bool initialize() = 0;
	virtual bool run() = 0;
	virtual bool shutdown() = 0;
};