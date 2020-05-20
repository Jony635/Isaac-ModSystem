sprite = "collectibles_360_incubus.png"

incubusOBJ = nil

positions = {}
delay = 0.15

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
	
end

function OnPlayerShoot(dir)

	local initialPosition = GetPosition(incubusOBJ)

	local newTear = {id = AddChild(), direction = dir}
	SetPosition(newTear.id, {x = initialPosition.x, y = initialPosition.y})
	SetComponent(newTear.id, "TearController")
	AddForce(newTear.id, dir, "Impulse")

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