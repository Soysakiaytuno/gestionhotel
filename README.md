# Gestion del Hotel

El sistema esta diseñado para un recepcionista de hotel, por ende solo se centrara en el respectivo registro de las reservas y estadias, validadndo lo que es el check in y check out, todo lo que puede hacer el sistema esta descrito con puntos mas detallados a continuacion: 

# 1. Alcance del Proyecto

* **Gestión del Ciclo de Estadía:** El sistema es capaz de hacer el registro de huespedes, en sus respectivas reservas para las estadias, donde seleccionaran la o las habitaciones y quienes van a estar durante esa estadia

* **Control del Flujo (Check-in / Check-out):** El sistema puede registrar el momento donde el cliente llega al registro y empieza su estadia, y el momento en donde se va el cliente

* **Cálculo de Tarifas Integrado:** Las tarifas se calcularan conforme el tiempo que hayan estado en el hotel, haciendo ese calculo y guardandolo posteriormente.

# Fuera del Alcance:

* **Autenticación:** No se hara la gestion de roles, login, ni encriptación de contraseñas.

* **Pagos:** El sistema no hara el proceso de tarjetas, ni pagos electrónicos ni manuales.

* **Facturación Electrónica:** No habra una forma de facturacion, solo se hara el calculo respectivo del pago, pero no se hace la gestion de facturacion.

* **Gestion de Clientes:** El sistema no va a poder registrar ni manejar huespedes, solo los utiliza para el registro, pero no puedes hacer gestion de estos.

# 2. Historias de usuario

* **RF01:** Consulta de Disponibilidad de Habitaciones: Como recepcionista, quiero buscar habitaciones disponibles en un rango de fechas, para poder asignarlas a una nueva reserva.

  - **CA1:** Dado que estoy en la pantalla de reservas, cuando ingreso una fecha de inicio y fin, entonces el sistema muestra una lista de las habitaciones disponibles indicando su número, tipo y capacidad.

  - **CA2:** Dado que una habitación está ocupada o reservada en las fechas solicitadas, cuando el sistema carga los resultados, entonces esa habitación no debe aparecer en la lista de selección.

  - **CA3:** Dado que un grupo necesita más de un cuarto, cuando selecciono varias habitaciones disponibles de la lista, entonces el sistema permite agruparlas para vincularlas a una sola estadía en proceso de creación.


* **RF02:** Búsqueda de Personas (Huéspedes): Como recepcionista, quiero buscar y seleccionar huéspedes previamente registrados en la base de datos, para poder añadirlos a la reserva.

  - **CA1:** Dado que necesito vincular un huésped a la estadía, cuando ingreso su documento de identidad o nombre en el buscador, entonces el sistema muestra los datos del usuario correspondiente para seleccionarlo.

  - **CA2:** Dado que el sistema no permite crear nuevos clientes, cuando busco un documento que no existe en la base de datos, entonces el sistema muestra un mensaje indicando que no hay resultados y bloquea la selección.


* **RF03:** Creación de Estadía (Programación): Como recepcionista, quiero crear una nueva estadía vinculando huéspedes, habitaciones y fechas, para registrarla oficialmente en el sistema.

  - **CA1:** Dado que he seleccionado fechas válidas, al menos una habitación disponible y al menos un huésped titular, cuando confirmo la acción, entonces el sistema guarda la estadía con el estado inicial "Programada".

  - **CA2:** Dado que intento guardar la estadía, cuando no he seleccionado ninguna habitación o falta el huésped titular, entonces el sistema muestra un error y no permite guardarla.


* **RF04:** Panel de Recepción (Dashboard): Como recepcionista, quiero visualizar un panel general de estadías, para identificar rápidamente los check-ins y check-outs del día.

  - **CA1:** Dado que ingreso al sistema, cuando visualizo el panel principal, entonces veo una lista de las estadías Programadas y en estado En Curso.

  - **CA2:** Dado que ubico una estadía en el listado, cuando esta se encuentra "Programada" o "En Curso", entonces el sistema me muestra los botones para accionar el Check-in o Check-out correspondientemente.


* **RF05:** Ejecución de Check-In: Como recepcionista, quiero marcar el check-in de una estadía programada, para confirmar la llegada de los huéspedes y el inicio real de la ocupación.

  - **CA1:** Dado que tengo una estadía en estado "Programada", cuando ejecuto la acción de Check-In, entonces el estado cambia a "En Curso" y el sistema registra automáticamente la fecha y hora actual como inicio real.

  - **CA2:** Dado que una estadía ya se encuentra "En Curso" o "Finalizada", cuando intento realizar la acción de Check-In, entonces el sistema no me muestra la opción.


* **RF06:** Ejecución de Check-Out y Cálculo de Cobro: Como recepcionista, quiero registrar el check-out de una estadía, para liberar las habitaciones y calcular automáticamente el monto a pagar.

  - **CA1:** Dado que una estadía está "En Curso", cuando ejecuto la acción de Check-Out, entonces el estado cambia a "Finalizada" y se registra la fecha y hora actual como salida real.

  - **CA2:** Dado que confirmo el Check-Out, cuando el sistema guarda el registro, entonces calcula automáticamente la cantidad del cobro que se va a realizar
