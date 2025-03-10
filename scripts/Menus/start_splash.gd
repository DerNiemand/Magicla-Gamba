extends Control

func _input(event):
	if not $"AnimationPlayer".is_playing():
		print(event.as_text())
		if event is not InputEventMouseMotion:
			get_tree().change_scene_to_file("res://Scenes/Menus/main_menu.tscn")
