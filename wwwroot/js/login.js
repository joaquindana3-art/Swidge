function validarLogin() {
  const usuario = document.getElementById("usuario").value.trim();
  const contrasena = document.getElementById("contrasena").value;
  const feedback = document.getElementById("fb-login");
 
  if (usuario === "" || contrasena === "") {
    feedback.textContent = "El usuario y la contraseña son obligatorios.";
    feedback.style.color = "red";
    return false; // Evita que el formulario se envíe
  }
 
  feedback.textContent = "";
  return true;
}
 