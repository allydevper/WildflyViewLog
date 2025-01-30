# WildflyViewLog

WildflyViewLog es una aplicación de escritorio desarrollada con Avalonia UI para visualizar y filtrar logs de Wildfly.

![alt text](https://raw.githubusercontent.com/allydevper/WildflyViewLog/refs/heads/main/show.png)

## Características

- Abrir archivos de log en formato `.txt`.
- Filtrar mensajes de log por contenido.
- Navegar a la primera coincidencia del filtro aplicado.
- Guardar logs filtrados.
- Interfaz de usuario moderna y responsiva.

## Requisitos

- .NET 8.0 o superior
- Avalonia UI 11.2.1
- CommunityToolkit.Mvvm 8.2.1
- MessageBox.Avalonia 3.2.0
- Newtonsoft.Json 13.0.3

## Instalación

1. Clona el repositorio:
    ```sh
    git clone https://github.com/tu-usuario/WildflyViewLog.git
    ```

2. Navega al directorio del proyecto:
    ```sh
    cd WildflyViewLog
    ```

3. Restaura las dependencias del proyecto:
    ```sh
    dotnet restore
    ```

4. Compila el proyecto:
    ```sh
    dotnet build
    ```

## Uso

1. Ejecuta la aplicación:
    ```sh
    dotnet run
    ```

2. Usa el botón "Abrir Archivo" para seleccionar un archivo de log en formato `.txt`.

3. Usa el campo de texto "Filtrar mensajes" para ingresar el texto que deseas filtrar y haz clic en "Aplicar Filtro".

4. Navega a la primera coincidencia del filtro aplicado en el `DataGrid`.

## Contribuciones

Las contribuciones son bienvenidas. Por favor, abre un issue o un pull request para discutir cualquier cambio que desees realizar.

## Licencia

Este proyecto está licenciado bajo la Licencia MIT. Consulta el archivo [LICENSE](LICENSE) para más detalles.
