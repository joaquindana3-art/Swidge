const Dni = document.getElementById("dni");
const Nombre = document.getElementById("nombre");
const Apellido = document.getElementById("apellido");
const NombreUsuario = document.getElementById("nombreUsuario");
const FechaNacimiento = document.getElementById("fechaNacimiento");
const Ciudad = document.getElementById("ciudad");
const Mail = document.getElementById("mail");
const Contrasena = document.getElementById("contrasena");
const ConfirmarContrasena = document.getElementById("confirmarContrasena");
const divResultado = document.getElementById("resultado");

const REGEX_LETRAS = /^[a-zA-ZáéíóúÁÉÍÓÚñÑüÜ\s]+$/;
const REGEX_USUARIO = /^[a-zA-Z0-9._-]+$/;
const REGEX_MAIL = /^[^@\s]+@[^@\s]+\.[^@\s]+$/;

function registrarUsuario() {
  const dni = Dni.value.trim();
  const nombre = Nombre.value.trim();
  const apellido = Apellido.value.trim();
  const nombreUsuario = NombreUsuario.value.trim();
  const fechaNacimiento = FechaNacimiento.value;
  const ciudad = Ciudad.value.trim();
  const mail = Mail.value.trim();
  const contrasena = Contrasena.value;
  const confirmar = ConfirmarContrasena.value;

  const errores = [];
  limpiarFeedbacks();

  // DNI: 7 u 8 dígitos
  if (dni === "") {
    errores.push("El DNI es obligatorio.");
    mostrarError("fb-dni", "Campo obligatorio");
  } else if (!/^\d{7,8}$/.test(dni)) {
    errores.push("El DNI debe tener 7 u 8 dígitos.");
    mostrarError("fb-dni", "7 u 8 dígitos, solo números");
  } else {
    mostrarOk("fb-dni", "✓ OK");
  }

  // Nombre: solo letras y espacios
  if (nombre === "") {
    errores.push("El nombre es obligatorio.");
    mostrarError("fb-nombre", "Campo obligatorio");
  } else if (!REGEX_LETRAS.test(nombre)) {
    errores.push("El nombre solo puede contener letras y espacios.");
    mostrarError("fb-nombre", "Solo se permiten letras y espacios");
  } else if (nombre.length > 50) {
    errores.push("El nombre no puede superar los 50 caracteres.");
    mostrarError("fb-nombre", "Máximo 50 caracteres");
  } else {
    mostrarOk("fb-nombre", "✓ OK");
  }

  // Apellido: solo letras y espacios
  if (apellido === "") {
    errores.push("El apellido es obligatorio.");
    mostrarError("fb-apellido", "Campo obligatorio");
  } else if (!REGEX_LETRAS.test(apellido)) {
    errores.push("El apellido solo puede contener letras y espacios.");
    mostrarError("fb-apellido", "Solo se permiten letras y espacios");
  } else if (apellido.length > 50) {
    errores.push("El apellido no puede superar los 50 caracteres.");
    mostrarError("fb-apellido", "Máximo 50 caracteres");
  } else {
    mostrarOk("fb-apellido", "✓ OK");
  }

  // Nombre de usuario: 6 a 50 caracteres, sin @ ni espacios
  if (nombreUsuario === "") {
    errores.push("El nombre de usuario es obligatorio.");
    mostrarError("fb-nombreUsuario", "Campo obligatorio");
  } else if (nombreUsuario.length < 6 || nombreUsuario.length > 50) {
    errores.push("El nombre de usuario debe tener entre 6 y 50 caracteres.");
    mostrarError("fb-nombreUsuario", "Entre 6 y 50 caracteres");
  } else if (!REGEX_USUARIO.test(nombreUsuario)) {
    errores.push("El nombre de usuario solo puede tener letras, números, punto, guion y guion bajo.");
    mostrarError("fb-nombreUsuario", "Solo letras, números, . - _");
  } else {
    mostrarOk("fb-nombreUsuario", "✓ OK");
  }

  // Fecha de nacimiento: válida y mayor de 18
  if (fechaNacimiento === "") {
    errores.push("La fecha de nacimiento es obligatoria.");
    mostrarError("fb-fechaNacimiento", "Campo obligatorio");
  } else if (!fechaValida(fechaNacimiento)) {
    errores.push("La fecha de nacimiento no es válida.");
    mostrarError("fb-fechaNacimiento", "Fecha no válida");
  } else if (calcularEdad(fechaNacimiento) < 18) {
    errores.push("Tenés que ser mayor de 18 años para registrarte.");
    mostrarError("fb-fechaNacimiento", "Tenés que ser mayor de 18");
  } else {
    mostrarOk("fb-fechaNacimiento", "✓ OK");
  }

  // Ciudad
  if (ciudad === "") {
    errores.push("La ciudad es obligatoria.");
    mostrarError("fb-ciudad", "Campo obligatorio");
  } else if (ciudad.length > 50) {
    errores.push("La ciudad no puede superar los 50 caracteres.");
    mostrarError("fb-ciudad", "Máximo 50 caracteres");
  } else {
    mostrarOk("fb-ciudad", "✓ OK");
  }

  // Mail
  if (mail === "") {
    errores.push("El mail es obligatorio.");
    mostrarError("fb-mail", "Campo obligatorio");
  } else if (mail.length > 50 || !REGEX_MAIL.test(mail)) {
    errores.push("El mail no es válido (máximo 50 caracteres).");
    mostrarError("fb-mail", "Mail no válido");
  } else {
    mostrarOk("fb-mail", "✓ OK");
  }

  // Contraseña: más de 8 y hasta 72 caracteres
  if (contrasena === "") {
    errores.push("La contraseña es obligatoria.");
    mostrarError("fb-contrasena", "Campo obligatorio");
  } else if (contrasena.length <= 8) {
    errores.push("La contraseña debe tener más de 8 caracteres.");
    mostrarError("fb-contrasena", "Más de 8 caracteres");
  } else if (contrasena.length > 72) {
    errores.push("La contraseña no puede superar los 72 caracteres.");
    mostrarError("fb-contrasena", "Máximo 72 caracteres");
  } else {
    mostrarOk("fb-contrasena", "✓ OK");
  }

  // Confirmación: igual a la contraseña
  if (confirmar === "") {
    errores.push("Tenés que confirmar la contraseña.");
    mostrarError("fb-confirmarContrasena", "Campo obligatorio");
  } else if (confirmar !== contrasena) {
    errores.push("Las contraseñas no coinciden.");
    mostrarError("fb-confirmarContrasena", "No coincide con la contraseña");
  } else {
    mostrarOk("fb-confirmarContrasena", "✓ OK");
  }

  if (errores.length > 0) {
    divResultado.style.color = "red";
    divResultado.style.border = "1px solid red";
    divResultado.style.padding = "8px";
    divResultado.innerHTML = "<strong>No se pudo registrar:</strong><br>" + errores.join("<br>");
    return false; // Evita que el formulario se envíe si hay errores
  } else {
    divResultado.innerHTML = "";
    divResultado.style.border = "none";
    return true; // Permite que el formulario se envíe si no hay errores
  }
}

// Recibe "yyyy-MM-dd" (lo que da el input type="date")
function fechaValida(valor) {
  const partes = valor.split("-").map(Number);
  if (partes.length !== 3 || partes.some(isNaN)) return false;
  const [a, m, d] = partes;
  if (a < 1900) return false;

  const fecha = new Date(a, m - 1, d);
  // Si el día no existe (ej: 31/02) JavaScript lo corrige solo: se detecta comparando de nuevo.
  if (fecha.getFullYear() !== a || fecha.getMonth() !== m - 1 || fecha.getDate() !== d) return false;

  return fecha <= new Date(); // no puede ser futura
}

function calcularEdad(valor) {
  const [a, m, d] = valor.split("-").map(Number);
  const hoy = new Date();
  let edad = hoy.getFullYear() - a;
  const mesActual = hoy.getMonth() + 1;
  if (mesActual < m || (mesActual === m && hoy.getDate() < d)) {
    edad--;
  }
  return edad;
}

function mostrarError(id, msg) {
  const el = document.getElementById(id);
  el.innerHTML = msg;
  el.style.color = "red";
}

function mostrarOk(id, msg) {
  const el = document.getElementById(id);
  el.innerHTML = msg;
  el.style.color = "green";
}

function limpiarFeedbacks() {
  const ids = ["fb-dni", "fb-nombre", "fb-apellido", "fb-nombreUsuario", "fb-fechaNacimiento",
               "fb-ciudad", "fb-mail", "fb-contrasena", "fb-confirmarContrasena"];
  ids.forEach(id => document.getElementById(id).innerHTML = "");
}