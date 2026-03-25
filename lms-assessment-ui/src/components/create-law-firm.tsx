import Button from "@mui/material/Button";
import TextField from "@mui/material/TextField";
import { useFormik } from "formik";
import * as yup from "yup";
import type { CreateLawFirmRequest } from "../types/law-firm-types";

const validationSchema = yup.object({
  name: yup.string().trim().required().min(1).max(50),
  address: yup.string().trim().required().min(1).max(100),
  phoneNumber: yup.string().trim().required(),
  email: yup.string().trim().required().email(),
});

const CreateLawFirmForm = () => {
  const form = useFormik({
    enableReinitialize: true,
    initialValues: {
      name: "",
      address: "",
      phoneNumber: "",
      email: "",
    },
    validationSchema: validationSchema,
    onSubmit: async (values: CreateLawFirmRequest) => await createTask(values),
  });

  return (
    <div>
      <form onSubmit={form.handleSubmit}>
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
        />
        <Button color="primary" variant="contained" fullWidth type="submit">
          Submit
        </Button>
      </form>
    </div>
  );
};

export default CreateLawFirmForm;
