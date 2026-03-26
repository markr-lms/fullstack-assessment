import Button from "@mui/material/Button";
import TextField from "@mui/material/TextField";
import { useFormik } from "formik";
import * as yup from "yup";
import type { CreateLawFirmRequest } from "~/types/law-firm-types";
import useApi from "~/hooks/useApi";
import { useMutation } from "@tanstack/react-query";

const validationSchema = yup.object({
  name: yup.string().trim().required().min(1).max(50),
  address: yup.string().trim().required().min(1).max(100),
  phoneNumber: yup.string().trim().required(),
  email: yup.string().trim().required().email(),
});

export default function CreateLawFirmForm() {
  const { createLawFirm } = useApi();

  const { mutateAsync, isPending } = useMutation({
    mutationFn: (values: CreateLawFirmRequest) => createLawFirm(values),
  });

  const form = useFormik({
    enableReinitialize: true,
    initialValues: {
      name: "",
      address: "",
      phoneNumber: "",
      email: "",
    },
    validationSchema: validationSchema,
    onSubmit: async (values: CreateLawFirmRequest) => await mutateAsync(values),
  });

  return (
    <section id="add-firm">
      <TextField
        fullWidth
        id="name"
        name="name"
        label="Name"
        value={form.values.name}
        onChange={form.handleChange}
        onBlur={form.handleBlur}
        error={form.touched.name && Boolean(form.errors.name)}
        helperText={form.touched.name && form.errors.name}
        disabled={isPending}
      />
      <TextField
        fullWidth
        id="address"
        name="address"
        label="Address"
        value={form.values.address}
        onChange={form.handleChange}
        onBlur={form.handleBlur}
        error={form.touched.address && Boolean(form.errors.address)}
        helperText={form.touched.address && form.errors.address}
        disabled={isPending}
      />
      <TextField
        fullWidth
        id="phoneNumber"
        name="phoneNumber"
        label="Phone Number"
        value={form.values.phoneNumber}
        onChange={form.handleChange}
        onBlur={form.handleBlur}
        error={form.touched.phoneNumber && Boolean(form.errors.phoneNumber)}
        helperText={form.touched.phoneNumber && form.errors.phoneNumber}
        disabled={isPending}
      />
      <TextField
        fullWidth
        id="email"
        name="email"
        label="Email"
        value={form.values.email}
        onChange={form.handleChange}
        onBlur={form.handleBlur}
        error={form.touched.email && Boolean(form.errors.email)}
        helperText={form.touched.email && form.errors.email}
        disabled={isPending}
      />
      <Button
        color="primary"
        variant="contained"
        fullWidth
        type="submit"
        onClick={() => form.submitForm()}
        disabled={isPending}
      >
        Create Law Firm
      </Button>
    </section>
  );
}
