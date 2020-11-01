# Luna-Lab-Exporter

This tiny library was developed using `Unity 2020.1`.

## Usage

1. Create a scene or use the `Sample` scene in the project.
2. On the menu tap `Luna Lab` and then click `Export To HTML5`.
3. Choose a location and a name and the scene will be exported.
4. Because of `CORS` problems the library will attempt to start a small HTTP Server
5. In case the server doesn't work, an easy alternative is to use the [LiveServer](https://marketplace.visualstudio.com/items?itemName=ritwickdey.LiveServer) extension for `vscode`.


`<sup>Please take into consideration that the Sample scene was not created with performance in mind.</sup>`

## Supported Features

Very basic support for the following features:

- GameObject
- Transform
- MeshFilter
- MeshRenderer
- Camera
- Animation
- Light

## External Libraries

- [three.js](https://threejs.org/)
- JSONObject
- HttpFileServer

