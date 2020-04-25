sprite = "collectibles_172_sacrificialdagger.png"

daggers = {}

function OnEquipped()

	for i = 1, 3 do		
		daggers[i] = AddChild()
	end

end

function Update()
	


end

return
{
	OnEquipped = OnEquipped,
	Update = Update
}