sprite = "collectibles_172_sacrificialdagger.png"

passiveItems = 
{
	"SacrificialDagger.lua"
}

function Update()

	amount, enemies = GetRoomEnemies()

	if amount > 0 then
		for i = 1, amount do

			print(enemies[i])

		end
	end

end

return
{
	Update = Update
}