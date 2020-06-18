Metamesh
========

![gif](https://i.imgur.com/4ldi2dH.gif)
![gif](https://i.imgur.com/GXz7Q41.gif)

**Metamesh** is a Unity package that generates primitive meshes. It works as a
custom asset importer but doesn't import anything from an input file. Instead
of importing a file, it reads properties from a metafile and procedurally
generates a mesh.

At the moment, it only supports very basic primitive shapes.

- Plane
- Box
- Sphere (UV sphere)
- Icosphere

How To Install
--------------

This package uses the [scoped registry] feature to resolve package dependencies.
Please add the following sections to the manifest file (Packages/manifest.json).

[scoped registry]: https://docs.unity3d.com/Manual/upm-scoped.html

To the `scopedRegistries` section:

```
{
  "name": "Keijiro",
  "url": "https://registry.npmjs.com",
  "scopes": [ "jp.keijiro" ]
}
```

To the `dependencies` section:

```
"jp.keijiro.metamesh": "0.0.6"
```

After changes, the manifest file should look like below:

```
{
  "scopedRegistries": [
    {
      "name": "Keijiro",
      "url": "https://registry.npmjs.com",
      "scopes": [ "jp.keijiro" ]
    }
  ],
  "dependencies": {
    "jp.keijiro.metamesh": "0.0.6",
...
```
