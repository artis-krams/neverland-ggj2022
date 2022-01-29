extends Control

const PORT = 8070

var _mouse_start: Vector2
var _last_color: int = -1
var _color: int

func host():
	var peer = NetworkedMultiplayerENet.new()
	peer.create_server(PORT)
	get_tree().set_network_peer(peer)
	get_tree().connect("network_peer_connected", self, "_player_connected")
	
	_last_color = (_last_color + 1) % 8
	_set_color(_last_color)


func join():
	var peer = NetworkedMultiplayerENet.new()
	peer.create_client("127.0.0.1", PORT)
	get_tree().set_network_peer(peer)
	
	$Spinner.show()
	#$Instructions.hide()
	yield(get_tree(), "connected_to_server")
	$Spinner.hide()
	#$Instructions.show()


func _player_connected(id):
	_last_color = (_last_color + 1) % 8
	rpc_id(id, "_set_color", _last_color)
	for color in range(8):
		rpc_id(id, "_init_canvas_color", $Canvas.get_used_cells_by_id(color), color)


remotesync func _init_canvas_color(positions, color):
	for position in positions:
		$Canvas.set_cellv(position, color)


remotesync func _set_color(color):
	_color = color
