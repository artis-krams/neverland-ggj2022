tool
extends Panel

signal clicked(instance)

export(bool) var filled = false setget set_filled
export(Texture) var texture = null setget set_texture
export(int, "32px", "64px") var size = 0 setget _set_size
export(int, "None", "Left", "Center", "Right") var group = 0 setget set_group

var _unfilled_stylebox = preload("Unfilled.tres")
var _filled_stylebox = preload("Filled.tres")

func _ready():
	call_deferred("_update_label_size")


func set_group(new_group):
	group = new_group
	if group == 0:
		_unfilled_stylebox = preload("Unfilled.tres")
		_filled_stylebox = preload("Filled.tres")
	elif group == 1:
		_unfilled_stylebox = preload("UnfilledLeft.tres")
		_filled_stylebox = preload("FilledLeft.tres")
	elif group == 2:
		_unfilled_stylebox = preload("UnfilledCenter.tres")
		_filled_stylebox = preload("FilledCenter.tres")
	else:
		_unfilled_stylebox = preload("UnfilledRight.tres")
		_filled_stylebox = preload("FilledRight.tres")
	set_filled(filled)

func _set_size(new_size):
	size = new_size
	var font
	if size == 0:
		font = preload("res://resources/fonts/RobotoLight-32px.tres")
	else:
		font = preload("res://resources/fonts/RobotoLight-64px.tres")
	
	set_filled(filled)
	call_deferred("_update_label_size")


func _update_label_size():
	$Texture.rect_size = rect_size
	$Texture.rect_pivot_offset = rect_size * 2.0


func set_texture(new_texture):
	texture = new_texture
	if texture:
		$Texture.texture = texture
		$Texture.show()
	else:
		$Texture.hide()
	

func set_filled(new_filled):
	filled = new_filled

	
	if filled:
		#$Texture.modulate = Color.white
		set("custom_styles/panel", _filled_stylebox)
	else:
		#$Texture.modulate = Color.black
		set("custom_styles/panel", _unfilled_stylebox)
		


func _on_Button_mouse_entered():
	if filled:
		modulate = Color.white * 1.05
	else:
		modulate = Color.white * 0.95
		


func _on_Button_mouse_exited():
	modulate = Color.white

func _on_Button_gui_input(event):
	if event is InputEventMouseButton:
		if event.pressed:
			if filled:
				modulate = Color.white * 1.3
			else:
				modulate = Color.white * 0.7
		else:
			_on_Button_mouse_entered()
			emit_signal("clicked", self)




func _on_Button_resized():
	_update_label_size()
