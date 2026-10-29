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

document.addEventListener('DOMContentLoaded', () => {
  const sectorSelect = document.getElementById('sectorEntrada');
  const filaGroup = document.getElementById('filaGroup');
  const asientoGroup = document.getElementById('asientoGroup');
  const form = document.querySelector('form[action*="PublicarEntrada"]');
  const eventoBusqueda = document.getElementById('eventoBusqueda');
  const eventoSeleccionadoId = document.getElementById('eventoSeleccionadoId');
  const eventosLista = document.getElementById('eventosLista');

  if (eventoBusqueda && eventosLista && eventoSeleccionadoId) {
    eventoBusqueda.addEventListener('input', () => {
      const valor = eventoBusqueda.value.trim();
      let idSeleccionado = '';

      for (const option of eventosLista.options) {
        if (option.value === valor) {
          idSeleccionado = option.dataset.id || '';
          break;
        }
      }

      eventoSeleccionadoId.value = idSeleccionado;
    });
  }

  if (!sectorSelect || !filaGroup || !asientoGroup || !form) return;

  const toggleSectorFields = () => {
    const esCampo = sectorSelect.value === 'Campo';

    filaGroup.style.display = esCampo ? 'none' : 'block';
    asientoGroup.style.display = esCampo ? 'none' : 'block';

    if (esCampo) {
      const filaInput = filaGroup.querySelector('input');
      const asientoInput = asientoGroup.querySelector('input');
      if (filaInput) filaInput.value = '';
      if (asientoInput) asientoInput.value = '';
    }
  };

  sectorSelect.addEventListener('change', toggleSectorFields);
  toggleSectorFields();

  form.addEventListener('submit', (event) => {
    const sector = sectorSelect.value;
    const fila = document.getElementById('fila')?.value.trim() ?? '';
    const asiento = document.getElementById('asiento')?.value.trim() ?? '';
    const feedback = document.getElementById('fb-publicacion');

    if (!eventoSeleccionadoId.value) {
      event.preventDefault();
      feedback.textContent = 'Debes seleccionar un evento existente.';
      feedback.style.display = 'block';
      feedback.style.color = 'red';
      return;
    }

    if (!sector) {
      event.preventDefault();
      feedback.textContent = 'Debes seleccionar un sector.';
      feedback.style.display = 'block';
      feedback.style.color = 'red';
      return;
    }

    if (sector !== 'Campo') {
      if (!fila || !asiento) {
        event.preventDefault();
        feedback.textContent = 'Si el sector no es Campo, fila y asiento son obligatorios.';
        feedback.style.display = 'block';
        feedback.style.color = 'red';
        return;
      }
    }

    feedback.textContent = '';
    feedback.style.display = 'none';
  });

  const fileInput = document.getElementById('comprobante');
  const preview = document.getElementById('imgPreview');
  const previewContainer = document.getElementById('previewComprobante');
  const placeholder = document.getElementById('placeholderComprobante');
  const botonCambiar = document.getElementById('btnCambiarArchivo');

  if (!fileInput || !preview || !previewContainer || !placeholder || !botonCambiar) return;

  fileInput.addEventListener('change', () => {
    const file = fileInput.files && fileInput.files[0];

    if (!file) {
      preview.style.display = 'none';
      previewContainer.classList.add('d-none');
      placeholder.classList.remove('d-none');
      botonCambiar.classList.add('d-none');
      return;
    }

    const url = URL.createObjectURL(file);
    preview.src = url;
    preview.style.display = 'block';
    previewContainer.classList.remove('d-none');
    placeholder.classList.add('d-none');
    botonCambiar.classList.remove('d-none');
  });

  botonCambiar.addEventListener('click', () => fileInput.click());
});
