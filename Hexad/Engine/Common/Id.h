#pragma once
#include "CommonHeaders.h"
#include <typeinfo>

namespace hexad::id {

	using id_type = u32;

	namespace detail {
		constexpr u32 generation_bits{ 8 }; // INCREASE OR FIND SOLUTION IF HAVING ROLL-OVER ISSUES
		constexpr u32 index_bits{ sizeof(id_type) * 8 - generation_bits };
		constexpr id_type index_mask{ (id_type{1} << index_bits) - 1 };
		constexpr id_type generation_mask{ (id_type{1} << generation_bits) - 1 };
	} // detail namepspace

	constexpr id_type invalid_id{ id_type(-1) };
	constexpr u32 min_deleted_elements{ 1024 };

	// Determines how many bits to use for generation
	using generation_type = std::conditional_t<detail::generation_bits <= 16, std::conditional_t<detail::generation_bits <= 8, u8, u16>, u32>;
	static_assert(sizeof(generation_type) * 8 >= detail::generation_bits);
	static_assert((sizeof(id_type) - sizeof(generation_type)) > 0);

	// checks if an ID is valid
	constexpr bool
	is_valid(id_type id) 
	{
		return id != invalid_id; // valid if not -1
	}

	// Masks the generation and gives only the index of the ID
	constexpr id_type
	index(id_type id) 
	{
		id_type index{ id & detail::index_mask };
		assert(index != detail::index_mask);
		return id & detail::index_mask;
	}

	// Masks the index and gives only the generation number of the ID
	constexpr id_type
	generation(id_type id)
	{
		return (id >> detail::index_bits) & detail::generation_mask; // shifts ID to the left to be left with only generation bits, and masks index
	}

	// Increment the generation part of the data
	constexpr id_type
	new_generation(id_type id)
	{
		const id_type generation{ id::generation(id) + 1 };
		assert(generation < (((u64)1 << detail::generation_bits) - 1));
		return index(id) | (generation << detail::index_bits);
	}

#if _DEBUG
	namespace detail {
		struct id_base
		{
			constexpr explicit id_base(id_type id) : _id{ id } {}
			constexpr operator id_type() const { return _id; } // implicit conversion

		private:
			id_type _id;
		};
	}

// inherits from id_base
// 2 ctor's --> pass to base
// the 2nd initiates member variable with invalid index
#define DEFINE_TYPED_ID(name)									\
		struct name final : id::detail::id_base				\
		{														\
			constexpr explicit name(id::id_type id)				\
				: id_base{ id } {}								\
			constexpr name() : id_base{ 0 } {}					\
		};
#else
#define DEFINE_TYPED_ID(name) using name = id::id_type;
#endif

}
