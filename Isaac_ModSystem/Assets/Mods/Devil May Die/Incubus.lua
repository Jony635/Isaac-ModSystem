sprite = "collectibles_360_incubus.png"

incubusOBJ = nil

positions = {}
delay = 1

tears = {}
maxTears = 20

function OnEquipped()

	incubusOBJ = AddChild()
	SetComponent(incubusOBJ, "SpriteRenderer", {sprite = 0})

	local IsaacPosition = GetPosition(0)
	SetPosition(incubusOBJ, {x = IsaacPosition.x, y = IsaacPosition.y})

end

function Update()

	local dt = GetDT()
	delay = delay - dt

	table.insert(positions, 1, GetPosition(0))

	if delay <= 0 then

		local newPosition = positions[#positions]
		table.remove(positions, #positions)

		SetPosition(incubusOBJ, {x = newPosition.x, y = newPosition.y})

	end

	for i, tear in ipairs(tears) do

		tearPosition = GetPosition(tear.id)
		SetPosition(tear.id, {x = tearPosition.x + (tear.direction.x * dt) , y = tearPosition.y + (tear.direction.y * dt)})

	end

end

function OnPlayerShoot(dir)

	local initialPosition = GetPosition(incubusOBJ)

	local newTear = {id = AddChild(), direction = dir}
	SetPosition(newTear.id, {x = initialPosition.x, y = initialPosition.y})
	SetComponent(newTear.id, "SpriteRenderer", {sprite = 0})

	table.insert(tears, newTear)

	if #tears > maxTears then

		DeleteChild(tears[1].id)
		table.remove(tears, 1)

	end
	
end


return
{
	OnEquipped = OnEquipped,
	Update = Update,
	OnPlayerShoot = OnPlayerShoot,
}