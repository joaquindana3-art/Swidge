document.addEventListener('DOMContentLoaded', () => {
  const form = document.getElementById('formPublicarEntrada');
  if (!form) return;

  const sectorSelect = document.getElementById('sectorEntrada');
  const filaGroup = document.getElementById('filaGroup');
  const asientoGroup = document.getElementById('asientoGroup');
  const eventoBusqueda = document.getElementById('eventoBusqueda');
  const eventoSeleccionadoId = document.getElementById('eventoSeleccionadoId');
  const eventosLista = document.getElementById('eventosLista');
  const feedback = document.getElementById('fb-publicacion');

  const mostrarErrores = (errores) => {
    if (!feedback) return;

    if (errores.length === 0) {
      feedback.textContent = '';
      feedback.style.display = 'none';
      return;
    }

    feedback.innerHTML = '<strong>Faltan datos:</strong><br>' + errores.join('<br>');
    feedback.style.color = 'red';
    feedback.style.display = 'block';
  };

  const validarFormulario = () => {
    const errores = [];

    if (!eventoSeleccionadoId || !eventoSeleccionadoId.value) {
      errores.push('Debes buscar y seleccionar un evento válido.');
    }

    if (!sectorSelect || !sectorSelect.value) {
      errores.push('Debes seleccionar un sector.');
    }

    if (sectorSelect && sectorSelect.value !== 'Campo') {
      const fila = document.getElementById('fila')?.value.trim() ?? '';
      const asiento = document.getElementById('asiento')?.value.trim() ?? '';

      if (!fila || !asiento) {
        errores.push('Para este sector, fila y asiento son obligatorios.');
      }
    }

    const precio = document.getElementById('precioVenta')?.value.trim() ?? '';
    const precioNumero = Number(precio.replace(',', '.'));
    if (!precio || Number.isNaN(precioNumero) || precioNumero <= 0) {
      errores.push('El precio de la entrada debe ser mayor a 0.');
    }

    const comprobante = document.getElementById('comprobante')?.files?.[0] ?? null;
    if (!comprobante) {
      errores.push('Debes subir un comprobante.');
    }

    const aceptaTerminos = document.getElementById('aceptaTerminos')?.checked ?? false;
    if (!aceptaTerminos) {
      errores.push('Debes aceptar los términos y condiciones.');
    }

    return errores;
  };

  const toggleSectorFields = () => {
    if (!sectorSelect || !filaGroup || !asientoGroup) return;

    const esCampo = sectorSelect.value === 'Campo';
    filaGroup.style.display = esCampo ? 'none' : 'block';
    asientoGroup.style.display = esCampo ? 'none' : 'block';

    const filaInput = filaGroup.querySelector('input');
    const asientoInput = asientoGroup.querySelector('input');

    if (filaInput) {
      filaInput.disabled = esCampo;
      if (esCampo) filaInput.value = '';
    }

    if (asientoInput) {
      asientoInput.disabled = esCampo;
      if (esCampo) asientoInput.value = '';
    }
  };

  if (sectorSelect) {
    sectorSelect.addEventListener('change', toggleSectorFields);
    toggleSectorFields();
  }

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

  form.addEventListener('submit', (event) => {
    event.preventDefault();

    const errores = validarFormulario();
    mostrarErrores(errores);

    if (errores.length > 0) {
      return;
    }

    form.submit();
  });

  const fileInput = document.getElementById('comprobante');
  const preview = document.getElementById('imgPreview');
  const previewContainer = document.getElementById('previewComprobante');
  const placeholder = document.getElementById('placeholderComprobante');
  const botonCambiar = document.getElementById('btnCambiarArchivo');

  if (fileInput && preview && previewContainer && placeholder && botonCambiar) {
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
  }
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
