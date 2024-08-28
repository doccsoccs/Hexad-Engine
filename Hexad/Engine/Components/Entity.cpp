#include "Entity.h"
#include "Transform.h"
#include "Script.h"

namespace hexad::game_entity {

	namespace {

		utl::vector<transform::component>	transforms;
		utl::vector<script::component>		scripts;

		utl::vector<id::generation_type>	generations;
		utl::deque<entity_id>				free_ids;

	} // anon namespace --> can't be accessed outside this .cpp file

	entity create(entity_info info)
	{
		assert(info.transform); // all game entities must have a transform component
		if (!info.transform) return entity{};

		entity_id id;

		// Reuse an existing free slot in the array
		if (free_ids.size() > id::min_deleted_elements)
		{
			id = free_ids.front();
			assert(!is_alive(id));
			free_ids.pop_front();
			id = entity_id{ id::new_generation(id) };
			++generations[id::index(id)];
		}
		else // create new element at end of array
		{
			id = entity_id{ (id::id_type)generations.size() };
			generations.push_back(0);

			// Resize components arrays
			// don't call resize() so the number of memory allocations is lower
			transforms.emplace_back();
			scripts.emplace_back();
		}

		const entity new_entity{ id };
		const id::id_type index{ id::index(id) };

		// CREATE TRANSFORM COMPONENT !!!
		assert(!transforms[index].is_valid());
		transforms[index] = transform::create(*info.transform, new_entity);
		if (!transforms[index].is_valid()) return {};

		// CREATE SCRIPT COMPONENT !!!
		// check if the entity has a script component
		// if not, and there is a valid creator pointer, add one
		if (info.script && info.script->script_creator)
		{
			assert(!scripts[index].is_valid());
			scripts[index] = script::create(*info.script, new_entity);
			assert(scripts[index].is_valid());
		}

		return new_entity;
	}

	void remove(entity_id id)
	{
		const id::id_type index{ id::index(id) };
		assert(is_alive(id));

		if (scripts[index].is_valid())
		{
			script::remove(scripts[index]);
			scripts[index] = {}; // override removed script array data slot with an empty ID
		}

		// add a default component in the removed slot
		transform::remove(transforms[index]);
		transforms[index] = {};

		free_ids.push_back(id);
	}

	bool is_alive(entity_id id)
	{
		// Aquire generation and index of an entity's id
		// Check f the entity is valid, check if the index is within the length of possible generations
		// Return whether the generation for this entity equals the generation of the slot in the array for the entity
		// (if the generation is different then the entity was replaced in the data array)
		assert(id::is_valid(id));
		const id::id_type index{ id::index(id) };
		assert(index < generations.size());
		assert(generations[index] == id::generation(id));
		return (generations[index] == id::generation(id) && transforms[index].is_valid());
	}

	transform::component entity::transform() const
	{
		assert(is_alive(_id));
		const id::id_type index{ id::index(_id) };
		return transforms[index];
	}

	script::component entity::script() const
	{
		assert(is_alive(_id));
		const id::id_type index{ id::index(_id) };
		return scripts[index];
	}
}