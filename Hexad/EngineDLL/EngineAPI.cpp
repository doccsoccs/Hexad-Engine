#ifndef EDITOR_INTERFACE
#define EDITOR_INTERFACE extern "C" __declspec(dllexport)
#endif // editor interface

#include "CommonHeaders.h"
#include "Id.h"
#include "..\Engine\Components\Entity.h"
#include "..\Engine\Components\Transform.h"

using namespace hexad;

namespace {

	struct transform_component
	{
		// Get values from editor
		f32 position[3];
		f32 rotation[3]; // --> to quaternion
		f32 scale[3];

		// Convert for use by the engine
		transform::init_info to_init_info()
		{
			using namespace DirectX;
			transform::init_info info{};

			// position & scale
			memcpy(&info.position[0], &position[0], sizeof(f32) * _countof(position));
			memcpy(&info.scale[0], &scale[0], sizeof(f32) * _countof(scale));

			// Rotation
			XMFLOAT3A rot{ &rotation[0] };
			XMVECTOR quat{ XMQuaternionRotationRollPitchYawFromVector(XMLoadFloat3A(&rot)) }; // SIMD memory --> stack (next line)
			XMFLOAT4A rot_quat{};
			XMStoreFloat4A(&rot_quat, quat);
			memcpy(&info.rotation[0], &rot_quat.x, sizeof(f32) * _countof(info.rotation));

			return info;
		}
	};

	struct game_entity_descriptor
	{
		transform_component transform;
	};

	// puts an id in an entity_id which is held in an entity that is then returned
	game_entity::entity entity_from_id(id::id_type id)
	{
		return game_entity::entity{ game_entity::entity_id{id} };
	}
}

// converts the game entity descriptor from the editor to an entity_info struct 
// which is used to create the game entity in the engine
EDITOR_INTERFACE id::id_type CreateGameEntity(game_entity_descriptor* e)
{
	assert(e);
	game_entity_descriptor& desc{ *e };
	transform::init_info transform_info{ desc.transform.to_init_info() };
	game_entity::entity_info entity_info
	{
		&transform_info,
	};
	return game_entity::create(entity_info).get_id();
}

EDITOR_INTERFACE void RemoveGameEntity(id::id_type id)
{
	assert(id::is_valid(id));
	game_entity::remove(game_entity::entity_id{ id });
}