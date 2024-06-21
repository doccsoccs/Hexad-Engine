#include "Script.h"
#include "Entity.h"

namespace hexad::script {

	namespace {		
		utl::vector<detail::script_ptr>		entity_scripts;
		utl::vector<id::id_type>			id_mapping;
		utl::vector<id::generation_type>	generations;
		utl::vector<script_id>				free_ids;
	} // anon namespace

	// Returns a boolean that expresses whether or not a script is a valid existing entity
	// existance depends on matching generation indeces, non-null, and validity
	bool exists(script_id id)
	{
		assert(id::is_valid(id)); // assert the id is valid
		const id::id_type index{ id::index(id) }; // get the index of the script id
		assert(index < generations.size() && id_mapping[index] < entity_scripts.size());
		assert(generations[index] == id::generation(id)); // assert the generation ids match

		return (generations[index] == id::generation(id)) &&
			entity_scripts[id_mapping[index]] &&
			entity_scripts[id_mapping[index]]->is_valid();
	}

	// CREATE A SCRIPT COMPONENT
	component create(init_info info, game_entity::entity entity)
	{
		assert(entity.is_valid());
		assert(info.script_creator);

		script_id id{};
		if (free_ids.size() > id::min_deleted_elements) // if the number of free ids exceeds the minimum, reuse the free ids
		{
			id = free_ids.front();
			assert(!exists(id));
			free_ids.pop_back();
			id = script_id{ id::new_generation(id) };
			++generations[id::index(id)];
		}
		else // add and use new slots at the end of the array 
		{
			id = script_id{ (id::id_type)id_mapping.size() };
			id_mapping.emplace_back();
			generations.push_back(0);
		}

		assert(id::is_valid(id));
		entity_scripts.emplace_back(info.script_creator(entity));		// creates an instance and adds it to the back of the entity scripts array
		assert(entity_scripts.back()->get_id() == entity.get_id());		// assert that the script class' id matches the entity its attatched to
		const id::id_type index{ (id::id_type)entity_scripts.size() };
		id_mapping[id::index(id)] = index;
		return component{ id };
	}

	// REMOVE A SCRIPT COMPONENT
	void remove(component c)
	{
		assert(c.is_valid() && exists(c.get_id()));
		const script_id id{ c.get_id() };
		const id::id_type index{ id_mapping[id::index(id)] };
		const script_id last_id{ entity_scripts.back()->script().get_id() };	// get the last element of the script array's id
		utl::erase_unordered(entity_scripts, index);
		id_mapping[id::index(last_id)] = index;
		id_mapping[id::index(id)] = id::invalid_id;
	}
};