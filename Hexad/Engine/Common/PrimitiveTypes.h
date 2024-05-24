#pragma once
#include <stdint.h>

// unsigned integers
using u8 = uint8_t;		// 0 - 254, 0000 0000 - 1111 1111
using u16 = uint16_t;
using u32 = uint32_t;
using u64 = uint64_t;

// marks the maximum value of each uint as invalid
constexpr u8 u8_invalid_id{ 0xffffui8 };
constexpr u16 u16_invalid_id{ 0xffff'ffffi16 };
constexpr u32 u32_invalid_id{ 0xffff'ffff'ffffui32 };
constexpr u64 u64_invalid_id{ 0xffff'ffff'ffff'ffffui64 };

// signed integers
using i8 = int8_t;
using i16 = int16_t;
using i32 = int32_t;
using i64 = int64_t;

// floating points
using f32 = float;