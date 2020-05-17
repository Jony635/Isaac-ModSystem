

function OnNewRoomEntered(alreadyDefeated)

	if not alreadyDefeated then

		amount, enemies = GetRoomEnemies()

		if amount > 0 then
			for index, monsterID in ipairs(enemies) do
				Damage(monsterID, 20)
			end
		end

	end

end

return
{
	OnNewRoomEntered = OnNewRoomEntered
}