sprite = "collectibles_093_thegamekid.png"
numCharges = 6

active = false
activeDuration = 6
hitRatio = 1

function OnUsed()

	active = true

	SetInvincible(true)

	Wait(activeDuration, function() active = false SetInvencible(false) end)

end

function OnCharacterCollidedWithMonster(enemy)

	if active then
		-- check if you are in the exact frame when you do damage
	end

end

return
{
	OnUsed = OnUsed,
	OnCharacterCollidedWithMonster = OnCharacterCollidedWithMonster
}