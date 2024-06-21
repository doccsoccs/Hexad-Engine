#pragma once

#define USE_STL_VECTOR 1
#define USE_STL_DEQUE 1

#if USE_STL_VECTOR
#include <vector>
namespace hexad::utl {
	template<typename T>
	using vector = std::vector<T>;

	template<typename T> 
	void erase_unordered(std::vector<T>& v, size_t index)
	{
		if (v.size() > 1) // if the vector contains 2 or more elements
		{
			// swaps the element at the given index with the last element
			std::iter_swap(v.begin() + index, v.end() - 1);
			// deletes the (soon to be removed/empty) last element
			v.pop_back();
		}
		else
		{
			v.clear();
		}
	}
}
#endif

#if USE_STL_DEQUE
#include <deque>
namespace hexad::utl {
	template<typename T>
	using deque = std::deque<T>;
}
#endif

namespace hexad::utl {

	// TODO: implement custom containers
	//template<class T>
	//class vector {};

}