document.addEventListener('DOMContentLoaded', () => {
  const form = document.querySelector('form[action*="PublicarEntrada"]');

  if (!form) return;

  form.addEventListener('submit', (event) => {
    event.preventDefault();

    const nombreEvento = document.getElementById('nombreEvento')?.value.trim() ?? '';
    const categoriaEvento = document.getElementById('categoriaEvento')?.value.trim() ?? '';
    const fechaEvento = document.getElementById('fechaEvento')?.value ?? '';
    const ciudadEvento = document.getElementById('ciudadEvento')?.value.trim() ?? '';
    const direccionEvento = document.getElementById('direccionEvento')?.value.trim() ?? '';
    const precioEntrada = document.getElementById('precioEntrada')?.value.trim() ?? '';
    const cantidadEntradas = document.getElementById('cantidadEntradas')?.value.trim() ?? '';
    const imagenEvento = document.getElementById('imagenEvento')?.files?.[0] ?? null;
    const comprobante = document.getElementById('comprobante')?.files?.[0] ?? null;
    const descripcionEvento = document.getElementById('descripcionEvento')?.value.trim() ?? '';
    const aceptaTerminos = document.getElementById('aceptaTerminos')?.checked ?? false;

    const errores = [];

    if (!nombreEvento) {
      errores.push('El nombre del evento es obligatorio.');
    } else if (nombreEvento.length < 3 || nombreEvento.length > 100) {
      errores.push('El nombre del evento debe tener entre 3 y 100 caracteres.');
    }

    if (!categoriaEvento) {
      errores.push('Debes seleccionar una categoría.');
    }

    if (!fechaEvento) {
      errores.push('La fecha del evento es obligatoria.');
    } else {
      const fechaIngresada = new Date(fechaEvento);
      const hoy = new Date();
      if (Number.isNaN(fechaIngresada.getTime()) || fechaIngresada <= hoy) {
        errores.push('La fecha del evento debe ser válida y futura.');
      }
    }

    if (!ciudadEvento) {
      errores.push('La ciudad del evento es obligatoria.');
    } else if (ciudadEvento.length > 80) {
      errores.push('La ciudad no puede superar los 80 caracteres.');
    }

    if (!direccionEvento) {
      errores.push('La dirección del evento es obligatoria.');
    } else if (direccionEvento.length < 5 || direccionEvento.length > 150) {
      errores.push('La dirección debe tener entre 5 y 150 caracteres.');
    }

    if (!precioEntrada) {
      errores.push('El precio de la entrada es obligatorio.');
    } else if (Number.isNaN(Number(precioEntrada)) || Number(precioEntrada) <= 0) {
      errores.push('El precio debe ser un número mayor a 0.');
    }

    if (!cantidadEntradas) {
      errores.push('La cantidad de entradas es obligatoria.');
    } else if (!/^\d+$/.test(cantidadEntradas) || Number(cantidadEntradas) <= 0) {
      errores.push('La cantidad de entradas debe ser un número entero mayor a 0.');
    }

    if (!imagenEvento) {
      errores.push('Debes subir una imagen del evento.');
    }

    if (!comprobante) {
      errores.push('Debes subir un comprobante.');
    }

    if (!descripcionEvento) {
      errores.push('La descripción del evento es obligatoria.');
    } else if (descripcionEvento.length < 20 || descripcionEvento.length > 500) {
      errores.push('La descripción debe tener entre 20 y 500 caracteres.');
    }

    if (!aceptaTerminos) {
      errores.push('Debes aceptar los términos y condiciones.');
    }

    const feedback = document.getElementById("fb-publicacion");
    if (errores.length > 0) {
      feedback.innerHTML = '<strong>Errores:</strong><br>' + errores.join('<br>');
      feedback.style.color = "red";
      feedback.style.display = "block";
      return;
    }

    feedback.textContent = "";
    feedback.style.display = "none";
    form.submit();
  });
});

function getInputByIdOrName(idOrNameList) {
  for (const value of idOrNameList) {
    const byId = document.getElementById(value);
    if (byId) return byId;

    const byName = document.querySelector(`[name="${value}"]`);
    if (byName) return byName;
  }

  return null;
}

function getFieldValue(idOrNameList) {
  const field = getInputByIdOrName(idOrNameList);
  if (!field) return "";

  if (field.type === "checkbox") return field.checked ? "ok" : "";
  if (field.type === "file") return field.files && field.files.length > 0 ? "ok" : "";

  return field.value ? field.value.trim() : "";
}

function validarPublicacion() {
  const nombreEvento = getFieldValue(["nombreEvento"]);
  const categoriaEvento = getFieldValue(["categoriaEvento"]);
  const fechaEvento = getFieldValue(["fechaEvento"]);
  const ciudadEvento = getFieldValue(["ciudadEvento"]);
  const direccionEvento = getFieldValue(["direccionEvento"]);
  const precioEntrada = getFieldValue(["precioEntrada"]);
  const cantidadEntradas = getFieldValue(["cantidadEntradas"]);
  const descripcionEvento = getFieldValue(["descripcionEvento"]);
  const aceptaTerminos = getInputByIdOrName(["aceptaTerminos"])?.checked ?? false;
  const feedback = document.getElementById("fb-publicacion");

  if (
    nombreEvento === "" ||
    categoriaEvento === "" ||
    fechaEvento === "" ||
    ciudadEvento === "" ||
    direccionEvento === "" ||
    precioEntrada === "" ||
    cantidadEntradas === "" ||
    descripcionEvento === "" ||
    !aceptaTerminos
  ) {
    if (feedback) {
      feedback.textContent = "Faltan datos obligatorios para publicar la entrada.";
      feedback.style.color = "red";
      feedback.style.display = "block";
    }
    return false;
  }

  if (feedback) {
    feedback.textContent = "";
    feedback.style.display = "none";
  }

  return true;
}
