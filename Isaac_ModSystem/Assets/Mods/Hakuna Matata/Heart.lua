sprite = "collectibles_015_heart.png"

function OnEquipped()

	SetMaxHealth(GetMaxHealth() + 2)
	SetHealth(GetMaxHealth())

end

return 
{
	OnEquipped = OnEquipped,
}