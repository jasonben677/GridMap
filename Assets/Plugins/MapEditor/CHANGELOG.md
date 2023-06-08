## CHANGELOG
### V1.7.1
- Add sorting group component on MeshRoot
### V1.7.0
- click unity tilemap cause toolwindow layer problem fixed
### V1.6.2
- npc pos change to string format
### V1.6.1
- Fixed load json error(npc)
### V1.6.0
- NPC can set face direction on tileobj
### V1.5.0
- Change teleport mapid(server use) ex : 99001 => 99000
### V1.4.2
- Change some error notify(red) to warning type(yellow)

### V1.4.1
- Fix unity scence data LF CRLF error

### v1.4.0
- Separate workarea to reduce rendering cost
- When clear or load a huge data will not do it in one frame 

### v1.3.1
- Put all MapEditor in UNITY_EDITOR Platform 

### v1.3.0
- Import old data will auto change the value on inspector
- Before click reset the editor will not clear data
- Force gridheight equal gridwidth when you change maptype to hexagonal

### v1.2.2
- Modified TileMapSceneEditor in Update() to comment, Because non-use.
    ```csharp
	private void Update()
	{
		//SceneModeUtility.SearchForType(typeof(TileMap));
	}
    ```
	
### v1.2.1
- Modified preprocessor tag UNITY_STANDALONE_WIN to MAP_EDITOR for FileManager and IMapBase.

### v1.2.0
- Fixed Undo and Redo feature, specially rectangle.
- Modified name of "padding" change to "margin".
- Modified ToolPicker has tooltip and shorten tile name.
- Modified name of example resource and organization.
- Modified MapEditor inspector name of export type.
- Modified when in MapEditor mode to change grid size will alert warning (only Exit MapEditor mode can change).
- Add new feature while export type is json has format option.
- Add new feature export will auto create a folder in accordance with export type (current there are json and o).

### v1.1.1
- Let material's texture fit grid.

### v1.1.0
- Change spriteRenderer to MeshRenderer.
- Remember import and export path.

### v1.0.0
- A unity map editor allow you set node status.