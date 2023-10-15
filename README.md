# rio-bcl

Various systems and helpers that I use throughout a number of my code bases. Most of this is currently undocumented. 

### BCL
- 'Common' project in my BCL
- Contains generic/simple implentation of common patterns (Command, Singleton, Pooling, etc.)
- Extension methods
- Internal logging for Unity projects
- Units of work
- Asynchronous systems; typically used for wrapping asynchronous systems

### BCL.Serialization
- Serializer used in my procedural generator
- Serializes JSON, text, prefabs

### BCL.Unmanaged
- Contains unmanaged code and unsafe contexts
- Used in procedural generation

### Curves
- For use in Unity projects
- Used in procedural generation, pathfinding, drawining, and line renders
- Can created lines, curves, & splines with various charcteristics
- Custom editor for visualization within Unity projects
- All data can be serialized and exported/imported

### Curves.Editor
- Editor for Curves library

### Procedural
- Codebase for procedural generator
- ONLY contains code for importing into other Unity projects

### Procedural.Editor
- Editor for Procedural library

### ProceduralAuxiliary
- Systems that are used within procedural generation, but can also be used standalone in other .NET/Unity projects

### Test.Benchmark
- Uses Benchmark.NET to benchmark libraries

### Test.Code
- Unit & integration tests for base class libraries

### UnityBCl
- Base class library specific to Unity projects
- This base class focuses HEAVILY on event driven & asynchronous Unity projects
- Addressables wrapper
- Unity specific collections
- Asynchronous code wrappers for UniTask library
- Serialized meshes, vector2 & vector3
- Monobehavior Singleton
- Debug.Log wrapper (UnityLogging)
- Pooling & scheduling abstractions
- Pooling & scheduling system
- Events for update, fixed update, and late update
- UniTask, Vector, Color, Debug, and string extensions
- Asynchronous colliders & triggers
- UI
- A number of utility & helper classes

### UnityBCL.Editor
- Editor wrapper for console logging
- Editor for searching & filtering objects (a bit obsolete with newer versions of Unity)
- Layers & tags defintions for new projects

### UnityBCL.Native
- Native classes for the job system & burst compiler

### UnityBCL.Serializtion
- A monobehavior wrapper for BCL.Serialization