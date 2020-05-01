sprite = "collectibles_110_momscontacts.png"
numCharges = 1

active = false
frozenEnemies = {}

function OnUsed()

	print("Im freezing you hahahahahaha")

end

function OnMonsterHittedByTear(enemy)


end

return
{
	OnUsed = OnUsed,
	OnMonsterHittedByTear = OnMonsterHittedByTear,
}