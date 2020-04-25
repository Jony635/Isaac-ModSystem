sprite = "collectibles_172_sacrificialdagger.png"

daggers = {}
angle = 0
radius = 4

function OnEquipped()

	for i = 1, 3 do		
		daggers[i] = AddChild()
	end

	position = GetPosition(0)
	print(string.format("x: %.2f	y: %.2f", position.x, position.y))

	SetPosition(0, {x = position.x - 3, y = position.y -3})
	
end

function Update()
	
	local dt = GetDT()

	angle = angle + dt * 30 --The angular speed

	if angle > 360 then
		angle = angle - 360
	end


end

return
{
	OnEquipped = OnEquipped,
	Update = Update
}