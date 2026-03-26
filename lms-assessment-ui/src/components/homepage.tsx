import Container from "@mui/material/Container";
import Divider from "@mui/material/Divider";
import Stack from "@mui/material/Stack";
import CreateLawFirmForm from "./create-law-firm";
import ListLawFirms from "./list-law-firms";

function Homepage() {
  return (
    <Container maxWidth="md">
      <header>
        <h1>LMS.Assessment</h1>
      </header>
      <Stack component="main" spacing={2}>
        <CreateLawFirmForm />
        <Divider />
        <ListLawFirms />
      </Stack>
    </Container>
  );
}

export default Homepage;
