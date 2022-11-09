# Physics-done-right
A project showing solutions to various physics issues.

## Implemented functionalities
* Player controller using new input system
  * Path visualisation
  * Acceleration-based
  * Air jumping
* Support for more complex environment
  * Physic material
  * Jump based on ground normal (including multiple ground normals) with bias
  * Limitation of movement based on ground angle
* Surface contact refinements
  * Ground snaping (bump prevention on small elevation differences) with configurable distance
  * Proper stairs handling (including props interacting with detailed collider and max stairs angle)
  * Steep contacts
* Orbit camera (3rd person)
  * Relative position
  * Focus radius
  * Manual rotation and automatic alignment
  * Camera input space
  * Camera collisions (BoxCast) with masking
  
## Gallery
![obraz](https://user-images.githubusercontent.com/107229318/200931711-e21f8f75-8c08-4c11-a95d-8b234c0e5858.png)
![obraz](https://user-images.githubusercontent.com/107229318/200931763-e0bd7ecb-ccb5-45ff-8916-734f7200171d.png)
![obraz](https://user-images.githubusercontent.com/107229318/200930349-9587d29d-78a2-4821-96e8-75b03df435df.png)
![obraz](https://user-images.githubusercontent.com/107229318/200930571-ee9477eb-9a97-4237-ab52-f1da44a025cb.png)
![obraz](https://user-images.githubusercontent.com/107229318/200930702-40465f25-ce61-467c-891a-cb4c116bbbbd.png)
![obraz](https://user-images.githubusercontent.com/107229318/200930777-4b5663b0-b7be-4e0f-a9de-03e6c116a433.png)
![obraz](https://user-images.githubusercontent.com/107229318/200930877-fb7ce8b8-729d-450e-a650-cd7c83d76737.png)
![obraz](https://user-images.githubusercontent.com/107229318/200930932-2e771b9f-1a80-46ee-8fa0-21ea0f9c9ef2.png)
![obraz](https://user-images.githubusercontent.com/107229318/200931058-8235fdc4-a47a-4814-ada1-167c3075b8b9.png)
  
## Scenes
* ComplexEnvironment - slope tests  
![obraz](https://user-images.githubusercontent.com/107229318/200929200-f64219c1-9db6-404b-a2d1-3528838af049.png)
* MultipleGroundNormals - jump and crevass test  
![obraz](https://user-images.githubusercontent.com/107229318/200929465-f3dd4f46-b601-489b-b653-0d97917c50f8.png)
* SurfaceContact - stairs and prop tests  
![obraz](https://user-images.githubusercontent.com/107229318/200929686-b1daab0f-34b3-43dd-9577-9de64a4228cd.png)
* CameraTests - orbit camera tests
![obraz](https://user-images.githubusercontent.com/107229318/200929768-c4fa2012-6dfa-4ec1-94fd-c2ba94df02cd.png)

## Used tools
* Probuilder

## Unity
Project created using version 2022.1.8f1
