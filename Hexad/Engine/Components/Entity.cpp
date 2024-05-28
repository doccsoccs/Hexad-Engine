#include "Entity.h"
#include "Transform.h"

namespace hexad::game_entity {

	namespace {

		utl::vector<transform::component> transforms;

		utl::vector<id::generation_type> generations;
		utl::deque<entity_id> free_ids;

	} // anon namespace --> can't be accessed outside this .cpp file

	entity create_game_entity(const entity_info& info)
	{
		assert(info.transform); // all game entities must have a transform component
		if (!info.transform) return entity{};

		entity_id id;

		// Reuse an existing free slot in the array
		if (free_ids.size() > id::min_deleted_elements)
		{
			id = free_ids.front();
			assert(!is_alive(entity{ id }));
			free_ids.pop_front();
			id = entity_id{ id::new_generation(id) };
			++generations[id::index(id)];
		}
		else // create new element at end of array
		{
			id = entity_id{ (id::id_type)generations.size() };
			generations.push_back(0);

			// Resize components array
			// don't call resize() so the number of memory allocations is lower
			transforms.emplace_back();
		}

		const entity new_entity{ id };
		const id::id_type index{ id::index(id) };

		// create transform component
		assert(!transforms[index].is_valid());
		transforms[index] = transform::create_transform(*info.transform, new_entity);
		if (!transforms[index].is_valid()) return {};

		return new_entity;
	}

	void remove_game_entity(entity e)
	{
		const entity_id id{ e.get_id() };
		const id::id_type index{ id::index(id) };
		assert(is_alive(e));
		if (is_alive(e)) 
		{
			// add a default component in the removed slot
			transform::remove_transform(transforms[index]);
			transforms[index] = {};

			free_ids.push_back(id);
		}
	}

	bool is_alive(entity e)
	{
		// Aquire generation and index of an entity's id
		// Check f the entity is valid, check if the index is within the length of possible generations
		// Return whether the generation for this entity equals the generation of the slot in the array for the entity
		// (if the generation is different then the entity was replaced in the data array)
		assert(e.is_valid());
		const entity_id id{ e.get_id() };
		const id::id_type index{ id::index(id) };
		assert(index < generations.size());
		assert(generations[index] == id::generation(id));
		return (generations[index] == id::generation(id) && transforms[index].is_valid());
	}

	transform::component entity::transform() const
	{
		assert(is_alive(*this));
		const id::id_type index{ id::index(_id) };
		return transforms[index];
	}
}