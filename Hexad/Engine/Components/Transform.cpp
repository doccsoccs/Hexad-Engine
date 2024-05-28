#include "Transform.h"
#include "Entity.h"

using namespace hexad::math;

namespace hexad::transform
{
	namespace {

		utl::vector<v3> positions;
		utl::vector<v4> rotations;
		utl::vector<v3> scales;

	}

	component create_transform(const init_info& info, game_entity::entity entity)
	{
		assert(entity.is_valid());
		const id::id_type entity_index{ id::index(entity.get_id()) };

		// If the component trying to added fits within the length of the entity array ...
		// override that slot with the new values (filling holes in the data array)
		if (positions.size() > entity_index)
		{
			rotations[entity_index] = v4(info.rotation);
			positions[entity_index] = v3(info.position);
			scales[entity_index] = v3(info.scale);
		}
		else 
		{
			assert(positions.size() == entity_index);
			rotations.emplace_back(info.rotation);
			positions.emplace_back(info.position);
			scales.emplace_back(info.scale);
		}

		return component(transform_id{ (id::id_type)positions.size() - 1});
	}

	void remove_transform(component c)
	{
		assert(c.is_valid());
	}

	// Gets the value of a given transform element
	v4 component::rotation() const
	{
		assert(is_valid());
		return rotations[id::index(_id)];
	}

	v3 component::position() const
	{
		assert(is_valid());
		return positions[id::index(_id)];
	}

	v3 component::scale() const
	{
		assert(is_valid());
		return scales[id::index(_id)];
	}
}