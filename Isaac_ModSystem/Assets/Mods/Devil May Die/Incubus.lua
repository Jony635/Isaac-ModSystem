sprite = "collectibles_360_incubus.png"

incubusOBJ = nil

function OnEquipped()

	incubusOBJ = AddChild()
	SetComponent(incubusOBJ, "SpriteRenderer", {sprite = 0})

end

function Update()

	if incubusOBJ ~= nil then

		IsaacPosition = GetPosition(0)
		SetPosition(incubusOBJ, {x = IsaacPosition.x, y = IsaacPosition.y - 1})

	end

end


return
{
	OnEquipped = OnEquipped,
	Update = Update
}