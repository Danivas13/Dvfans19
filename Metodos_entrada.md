# Metodo de entradas.

Los juegos basados Dirext como Fifa estan protegidos contra cheats y hacer modificaciones grandes
puede causar un baneo es por eso que esta aplicacion trata de ser lo menos invasiva posible.

La solución a esta limitacion por el momento es usar teclas de funcion combinadas con la tecla Ctrl
quedando los siguientes teclas establecidas

Ctrl + F2      =      Detectar equipos e iniciar servicio de cantos de hinchadas.
Ctrl + F3      =      Configurar HUD

# Capturas de Pantalla y sobreposición

Debido a que los juegos cuando se ejecutan en pantalla completa no pueden ser leidos por aplicaciones externas amenos que se 
haga una inyeción de .Dll Que puede traer como consecuencia  baneos cuando la aplicación no es autorizada por los dessarroladores.

La soluccion a esto ha sido ejecutar el juego En pantalla sin bordes en luego de pantalla completa.

EL juego seguira ocupando toda la pantalla pero permitira a otros aplicaciones funcionar
(Esto puede reducir un poco el desempeño del juego en Pc menos potentes)

### Metodo 1 En el directorio de instalacion ir a "FIFASetup/" y abrir el archivo fifaconfig.exe

<img src="Imagenes/Captura1.PNG?raw=true" width="450">
<img src="Imagenes/Captura2.PNG?raw=true" width="450">

### Metodo 2 En el directorio de Fifa 19 en documentos abrir el archivo fifasetup.ini y 
- Colocar un 0 en FULLSCREEN y colocar un 1 en WINDOWED_BORDERLESS
<img src="Imagenes/Captura3.PNG?raw=true" width="450">
# Configuración del HUD

El problema: La detencion de los equipos que estan jugando, el marcador y los goles anotados se realiza mediante
reconocimiento de textos OCR (Reconocimiento óptico de caracteres) esta labor se dificulta cuando en la imagen
capturada hay elementos que no sean textos o numeros y se relentiza entre mas grande sea la imagen.

El configurar el HUD nos da un area de recorte mas pequeña y nos permite crear una mascara de recortes que nos permitira
mejorar la calidad de la información obtenida mejorando el desempeño y la eficacia de la aplicacion.

## ¿Cuando y como se configura?

Se le solicita al usuario configurar el HUD cuando:
-Inicia por primera vez la aplicacion
-El archivo hudcords.Ini que almacena las cordenas del hud ha sido eliminado del directorio de la aplicacion

Adicionalmente el usuario debera configurarlo cada vez que cambie de monitor por uno de diferente resolucion.

### ¿Como?

Despues de presionar F3 aparecera un cuadro rojo que podra:

Moverse usando: 
Ctrl + ←
Ctrl + ↑
Ctrl + →
Ctrl + ←
Ctrl + ↓

Hacerse mas ancho con Ctrl + W, Menos ancho con Ctrl + Q

Hacerse mas alto con Ctrl + S, Menos alto con Ctrl + A

#### El cuadro de configuracion debe hacerse coincidir con el de calibración en el juego en:

"Personalizar - Configuración - Video - Calibrar Video"
[Insetar GIF Aqui de muestra]


#Inicio de la aplicación

Una vez abierto DVFANS19 y calibrado se debera presionar Ctrl + F2 en la pestaña de entrenamiento:

Ejemplo:

<br>
<img src="Imagenes/test161.png?raw=true" width="600">
<br>

O en el menu de pausa:

Ejemplo:
<br>
<img src="Imagenes/test114.png?raw=true" width="600">
<br>
