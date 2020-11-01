const renderer = new THREE.WebGLRenderer({ antialias: true });
renderer.shadowMap.enabled = true;
renderer.shadowMap.type = THREE.PCFSoftShadowMap;

const clock = new THREE.Clock();

let gameData = {};

function loadGame() {
  fetch("game_config.json")
    .then((response) => response.json())
    .then((json) => startGame(json));
}

function startGame(configJson) {
  const { width, height } = configJson.window;
  renderer.setSize(width, height);
  gameData.window = configJson.window;
  document.body.appendChild(renderer.domElement);

  createScenes(configJson);
  animate();
}

function createScenes(configJson) {
  gameData.scenesData = configJson.scenes.map((sceneJson) => {
    const sceneData = {
      scene: new THREE.Scene(),
      instanceRefs: {},
    };

    addObjectsTo(
      sceneData.scene,
      sceneJson.hierarchy,
      sceneJson.resources,
      sceneData.instanceRefs
    );

    Object.values(sceneData.instanceRefs).forEach((element) => {
      if (element instanceof THREE.Camera) sceneData.camera = element;
    });

    //Hacky way to to fix negative z for camera forward
    sceneData.camera.scale.z = -1;
    sceneData.scene.scale.z = -1;

    return sceneData;
  });
}

function addObjectsTo(container, objects, resources, instanceRefs) {
  objects.forEach((id) => {
    const object = resources[id];
    var group = new THREE.Group();
    addComponentsTo(group, object.components, resources, instanceRefs);
    addObjectsTo(group, object.children, resources, instanceRefs);
    group.visible = object.active;

    instanceRefs[object.instanceID] = group;
    container.add(group);
  });
}

function addComponentsTo(group, components, resources, instanceRefs) {
  components.forEach((id) => {
    const component = resources[id];
    var createComponent =
      {
        Transform: createTransform,
        Camera: createCamera,
        MeshFilter: createMeshFilter,
        MeshRenderer: createMeshRenderer,
        Light: createLight,
        Animation: createAnimation,
      }[component.type] || function () {};

    createComponent(component, group, resources, instanceRefs);
  });
}

function animate() {
  requestAnimationFrame(animate);
  const { scene, camera, instanceRefs } = gameData.scenesData[0];
  Object.values(instanceRefs).forEach(
    (e) => e.update && e.update(clock.getDelta())
  );

  renderer.render(scene, camera);
}

loadGame();

//
//
//Helpers
Array.prototype.chunk = function (n) {
  if (!this.length) return [];
  return [this.slice(0, n)].concat(this.slice(n).chunk(n));
};

function createTransform(transJson, group, resources, instanceRefs) {
  group.scale.fromArray(Object.values(transJson.localScale));
  group.position.fromArray(Object.values(transJson.localPosition));
  group.rotation.setFromQuaternion(
    new THREE.Quaternion().fromArray(Object.values(transJson.localRotation))
  );
  instanceRefs[transJson.instanceID] = group;
}

function createCamera(camJson, group, resources, instanceRefs) {
  const { width, height } = gameData.window;

  const colorRGB = new THREE.Color(...Object.values(camJson.backgroundColor));
  renderer.setClearColor(colorRGB);

  const camera = new THREE.PerspectiveCamera(
    camJson.fieldOfView,
    width / height,
    camJson.nearClipPlane,
    camJson.farClipPlane
  );

  group.add(camera);
  instanceRefs[camJson.instanceID] = camera;
}

function createMeshFilter(meshFilter, group, resources, instanceRefs) {
  const meshData = resources[meshFilter.mesh];
  const geometry = new THREE.Geometry();

  geometry.vertices = meshData.vertices.map(
    (e) => new THREE.Vector3(...Object.values(e))
  );
  geometry.faces = meshData.triangles
    .chunk(3)
    .map((e) => new THREE.Face3(...Object.values(e)));
  geometry.computeFaceNormals();
  geometry.computeVertexNormals();

  const material = new THREE.MeshStandardMaterial({ color: 0xffffff });
  const mesh = new THREE.Mesh(geometry, material);

  group.add(mesh);
  instanceRefs[meshFilter.instanceID] = mesh;
}

function createMeshRenderer(meshRenderJson, group, resources, instanceRefs) {
  const mesh = instanceRefs[meshRenderJson.meshFilter];
  const material = resources[meshRenderJson.material];

  const colorRGB = new THREE.Color(...Object.values(material.color));
  const emissionRBG = new THREE.Color(...Object.values(material.emissionColor));

  mesh.material.color = colorRGB;
  mesh.material.emissive = emissionRBG;
  mesh.material.metalness = material.metallic;
  mesh.material.roughness = 1 - material.glossiness;
  mesh.castShadow = meshRenderJson.castShadows;
  mesh.receiveShadow = meshRenderJson.receiveShadows;
}

function createLight(lightJson, group, resources, instanceRefs) {
  const { category, color, range, intensity, angle } = lightJson;
  const lightTypes = {
    Directional: THREE.DirectionalLight,
    Point: THREE.PointLight,
    Spot: THREE.SpotLight,
  };

  const colorRGB = new THREE.Color(...Object.values(lightJson.color));

  let light = new lightTypes[category](
    colorRGB,
    intensity,
    range || 0,
    (angle / 360) * Math.PI
  );

  if (light.target) {
    light.add(light.target);
    if (category === "Directional") light.position.set(0, 0, -20);
    light.target.position.set(0, 0, 100);
    light.target.updateMatrixWorld();
  }

  light.castShadow = lightJson.castShadows;
  group.add(light);
}

function createAnimation(animJson, group, resources, instanceRefs) {
  const clipJson = resources[animJson.clip];

  const tracks = clipJson.bindings.map((binding) => {
    const { times, values, propertyName } = binding;
    const name = {
      "m_LocalPosition.x": ".position[x]",
      "m_LocalPosition.y": ".position[y]",
      "m_LocalPosition.z": ".position[z]",

      "m_LocalRotation.x": ".quaternion[x]",
      "m_LocalRotation.y": ".quaternion[y]",
      "m_LocalRotation.z": ".quaternion[z]",
      "m_LocalRotation.w": ".quaternion[w]",
    }[propertyName];

    return new THREE.NumberKeyframeTrack(name, times, values);
  });
  const clip = new THREE.AnimationClip(null, -1, tracks);
  const mixer = new THREE.AnimationMixer(group);
  const clipAction = mixer.clipAction(clip);

  clipAction.play();
  instanceRefs[clipJson.instanceID] = mixer;
}
